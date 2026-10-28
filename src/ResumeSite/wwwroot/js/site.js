// Smooth scrolling, active-section tracking, frosted header and fade-in reveals.
// Loaded by PageScrollService via JS interop.

let dotNetRef = null;
let spyObserver = null;
let revealObserver = null;
let onScroll = null;
let currentId = null;
let sectionIds = [];

const prefersReducedMotion = () =>
  window.matchMedia('(prefers-reduced-motion: reduce)').matches;

function setActive(id) {
  if (!id || id === currentId) return;
  currentId = id;
  dotNetRef?.invokeMethodAsync('OnActiveSectionChanged', id);
}

export function initialize(ref, ids) {
  dispose();
  dotNetRef = ref;
  sectionIds = ids;

  // 1. Frosted header once the page has scrolled; last section active at the very bottom.
  const header = document.getElementById('site-header');
  onScroll = () => {
    header?.toggleAttribute('data-scrolled', window.scrollY > 8);
    const atBottom = window.innerHeight + window.scrollY >= document.documentElement.scrollHeight - 2;
    if (atBottom) setActive(sectionIds[sectionIds.length - 1]);
  };
  window.addEventListener('scroll', onScroll, { passive: true });
  onScroll();

  // 2. Scroll-spy: a section is active when it crosses a band 40–45% down the viewport.
  spyObserver = new IntersectionObserver(entries => {
    for (const entry of entries) {
      if (entry.isIntersecting) setActive(entry.target.id);
    }
  }, { rootMargin: '-40% 0px -55% 0px' });

  for (const id of sectionIds) {
    const el = document.getElementById(id);
    if (el) spyObserver.observe(el);
  }

  // 3. Fade-in on scroll. Content stays visible if JS fails or the user prefers reduced motion.
  const revealEls = document.querySelectorAll('.reveal');
  if (prefersReducedMotion()) {
    revealEls.forEach(el => el.classList.add('is-visible'));
  } else {
    document.documentElement.classList.add('reveal-ready');
    revealObserver = new IntersectionObserver((entries, observer) => {
      for (const entry of entries) {
        if (entry.isIntersecting) {
          entry.target.classList.add('is-visible');
          observer.unobserve(entry.target);
        }
      }
    }, { rootMargin: '0px 0px -10% 0px', threshold: 0.1 });
    revealEls.forEach(el => revealObserver.observe(el));
  }

  // 4. Opened with a link like /#about? Jump straight there.
  if (location.hash.length > 1) {
    const id = decodeURIComponent(location.hash.slice(1));
    if (document.getElementById(id)) scrollToSection(id, false, true);
  }
}

export function scrollToSection(id, updateHash = true, instant = false) {
  const el = document.getElementById(id);
  if (!el) return;

  el.scrollIntoView({
    behavior: instant || prefersReducedMotion() ? 'instant' : 'smooth',
    block: 'start'
  });

  // Move keyboard focus to the section so the next Tab continues from there.
  el.focus({ preventScroll: true });

  // Shareable URL (e.g. /#about) without triggering Blazor's router.
  if (updateHash) history.replaceState(history.state, '', `#${id}`);
}

export function dispose() {
  spyObserver?.disconnect();
  revealObserver?.disconnect();
  if (onScroll) window.removeEventListener('scroll', onScroll);
  spyObserver = revealObserver = onScroll = dotNetRef = currentId = null;
}