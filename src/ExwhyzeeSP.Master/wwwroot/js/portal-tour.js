/**
 * ExwhyzeeSP Dreams Core - Interactive Tour Guide Engine
 * Powered by Driver.js v1.3.1
 */

(function () {
    'use strict';

    // Normalize page key from current URL path
    function getPageKey() {
        let path = window.location.pathname.toLowerCase();
        // Strip trailing slashes
        path = path.replace(/\/+$/, '');
        if (!path || path === '') path = '/admin/dashboard';
        return path;
    }

    // Curated default steps for core portal pages (fallback if no HTML data-attributes are placed)
    const pageTourRegistry = {
        '/admin/dashboard': [
            {
                element: '#appSidebar',
                popover: {
                    title: 'Global Navigation Menu',
                    description: 'Easily navigate through Schools, Billing, Marketing, Analytics, and Platform Settings.',
                    side: 'right',
                    align: 'start'
                }
            },
            {
                element: '.header-search-wrapper',
                popover: {
                    title: 'Universal Instant Search',
                    description: 'Search school names, subdomains, domains, and administrative records on the fly.',
                    side: 'bottom',
                    align: 'start'
                }
            },
            {
                element: '.hs-dropdown:has(.icon-plus)',
                popover: {
                    title: 'Quick Actions Launchpad',
                    description: 'Quickly provision a new school, launch a marketing campaign, or generate billing invoices.',
                    side: 'bottom',
                    align: 'end'
                }
            },
            {
                element: 'main.main-content',
                popover: {
                    title: 'Executive Dashboard & Telemetry',
                    description: 'Live summaries of enrolled students, active school instances, revenue trends, and system health index.',
                    side: 'top',
                    align: 'center'
                }
            }
        ],
        '/admin/tenants': [
            {
                element: 'a[href*="/Tenants/Create"], button:has(.icon-plus), a:has(.icon-plus)',
                popover: {
                    title: 'Provision New School',
                    description: 'Onboard a new school instance with dedicated tenant subdomains and initial administrator credentials.',
                    side: 'bottom',
                    align: 'end'
                }
            },
            {
                element: 'table, .grid:has(.surface-card)',
                popover: {
                    title: 'School Tenants Directory',
                    description: 'Search, filter, view status, access delegated SSO login, or manage billing subscriptions for each school.',
                    side: 'top',
                    align: 'center'
                }
            }
        ],
        '/admin/analytics': [
            {
                element: '.grid.grid-cols-1.sm\\:grid-cols-2',
                popover: {
                    title: 'Cross-Tenant Analytics KPIs',
                    description: 'Aggregated total student population, academic faculty, billing ARR, and health ratings across all school nodes.',
                    side: 'bottom',
                    align: 'start'
                }
            },
            {
                element: '#enrollmentChart, #activityTimelineChart, .surface-card:has(#schoolComparisonChart)',
                popover: {
                    title: 'Multi-Tenant Growth & Trends',
                    description: 'Interactive ApexCharts visualizing student enrollment distributions and academic stage comparisons.',
                    side: 'top',
                    align: 'center'
                }
            }
        ],
        '/admin/analytics/demographics': [
            {
                element: '.surface-card:has(.icon-database)',
                popover: {
                    title: 'Data Provenance & Audit Card',
                    description: 'Transparency summary explaining how live metrics are queried and aggregated from MasterTenantMetricSnapshots.',
                    side: 'bottom',
                    align: 'start'
                }
            },
            {
                element: '#genderBreakdownChart',
                popover: {
                    title: 'Gender Distribution Analytics',
                    description: 'Real-time stacked comparison of male and female students across registered school campuses.',
                    side: 'top',
                    align: 'center'
                }
            },
            {
                element: '#stagesChart',
                popover: {
                    title: 'Academic Divisions Breakdown',
                    description: 'Visual breakdown across Early Years, Primary, Junior Secondary, and Senior Secondary tiers.',
                    side: 'left',
                    align: 'center'
                }
            }
        ],
        '/admin/analytics/telemetrylive': [
            {
                element: '.grid.grid-cols-1.sm\\:grid-cols-2',
                popover: {
                    title: 'Cluster Telemetry Overview',
                    description: 'Real-time telemetry indicators: average latency (ms), node storage (MB/GB), and heartbeat health.',
                    side: 'bottom',
                    align: 'start'
                }
            },
            {
                element: '#liveNodesContainer, table, .surface-card:has(#nodesTableBody)',
                popover: {
                    title: 'Live School Instance Feed',
                    description: 'Streaming node statuses updated in real time directly from school telemetry sync endpoints.',
                    side: 'top',
                    align: 'center'
                }
            }
        ],
        '/admin/marketing': [
            {
                element: 'a[href*="/Marketing/CreateCampaign"], a:has(.icon-send)',
                popover: {
                    title: 'Create & Broadcast Campaign',
                    description: 'Compose and dispatch rich marketing email broadcasts to segmented prospective leads.',
                    side: 'bottom',
                    align: 'end'
                }
            },
            {
                element: 'a[href*="/Marketing/Templates"]',
                popover: {
                    title: 'Email Templates Hub',
                    description: 'Design responsive email templates using the dual-mode Visual WYSIWYG and HTML Source editor.',
                    side: 'bottom',
                    align: 'center'
                }
            }
        ],
        '/admin/marketing/contacts': [
            {
                element: 'a[href*="/Marketing/Import"], a:has(.icon-upload)',
                popover: {
                    title: 'Bulk Lead Import',
                    description: 'Import leads and contact lists directly from Excel spreadsheets or CSV files.',
                    side: 'bottom',
                    align: 'end'
                }
            },
            {
                element: '.surface-card:has(table)',
                popover: {
                    title: 'Audience Directory & Segmentation',
                    description: 'Filter leads by category (Schools, Parents, Proprietors), subscription status, and engagement levels.',
                    side: 'top',
                    align: 'center'
                }
            }
        ],
        '/account/activitylog': [
            {
                element: '#masterFilterForm, .surface-card:has(#inputFromDate)',
                popover: {
                    title: 'Master Date Range & Scope Filter',
                    description: 'Filter audit logs with one-click quick presets (Today, Last 7/14/30 Days, All Time) or custom date pickers.',
                    side: 'bottom',
                    align: 'start'
                }
            },
            {
                element: '#activityTimelineChart',
                popover: {
                    title: 'Interactive Event Timeline',
                    description: 'Visual volume trends of successful logins, delegated SSO sessions, and security events over the selected period.',
                    side: 'top',
                    align: 'center'
                }
            },
            {
                element: 'button[data-bs-target="#purgeModal"]',
                popover: {
                    title: 'Log Archival & Storage Reclaim',
                    description: 'Purge historical records older than 30/90/180 days to keep your database lean and performant.',
                    side: 'bottom',
                    align: 'end'
                }
            }
        ]
    };

    // Generic fallback tour steps when no specific matches exist
    function getFallbackSteps() {
        const steps = [];

        const sidebar = document.querySelector('#appSidebar');
        if (sidebar) {
            steps.push({
                element: sidebar,
                popover: {
                    title: 'Navigation Sidebar',
                    description: 'Access all portal modules including Schools, Ledger, Marketing, and System Settings.',
                    side: 'right',
                    align: 'start'
                }
            });
        }

        const search = document.querySelector('.header-search-wrapper');
        if (search) {
            steps.push({
                element: search,
                popover: {
                    title: 'Quick Search',
                    description: 'Quickly find schools, administrators, and configuration records.',
                    side: 'bottom',
                    align: 'start'
                }
            });
        }

        const helpBtn = document.querySelector('#pageTourTriggerBtn');
        if (helpBtn) {
            steps.push({
                element: helpBtn,
                popover: {
                    title: 'Interactive Page Tour Guide',
                    description: 'You can replay this interactive walkthrough at any time by clicking this Help Tour button.',
                    side: 'bottom',
                    align: 'end'
                }
            });
        }

        const mainContent = document.querySelector('main.main-content');
        if (mainContent && steps.length < 3) {
            steps.push({
                element: mainContent,
                popover: {
                    title: 'Main Workspace',
                    description: 'View and manage live data records, analytics charts, and operational actions for this section.',
                    side: 'top',
                    align: 'center'
                }
            });
        }

        return steps;
    }

    // Collect steps from DOM data-tour-step attributes
    function collectDeclaredDOMSteps() {
        const elements = document.querySelectorAll('[data-tour-step]');
        if (!elements || elements.length === 0) return null;

        const sorted = Array.from(elements).sort((a, b) => {
            const stepA = parseInt(a.getAttribute('data-tour-step') || '0', 10);
            const stepB = parseInt(b.getAttribute('data-tour-step') || '0', 10);
            return stepA - stepB;
        });

        const steps = [];
        sorted.forEach(el => {
            const title = el.getAttribute('data-tour-title') || 'Page Feature';
            const desc = el.getAttribute('data-tour-desc') || 'Explore this section of the portal.';
            const side = el.getAttribute('data-tour-pos') || 'bottom';

            steps.push({
                element: el,
                popover: {
                    title: title,
                    description: desc,
                    side: side,
                    align: 'start'
                }
            });
        });

        return steps.length > 0 ? steps : null;
    }

    // Find and resolve steps for the active page
    function resolvePageSteps() {
        // 1. Declarative DOM tags
        const domSteps = collectDeclaredDOMSteps();
        if (domSteps && domSteps.length > 0) {
            return domSteps;
        }

        // 2. Curated Page Registry
        const pageKey = getPageKey();
        let registrySteps = pageTourRegistry[pageKey];

        // Also check partial path matching (e.g. /admin/tenants/details matches /admin/tenants)
        if (!registrySteps) {
            for (const key in pageTourRegistry) {
                if (pageKey.startsWith(key)) {
                    registrySteps = pageTourRegistry[key];
                    break;
                }
            }
        }

        if (registrySteps && registrySteps.length > 0) {
            // Filter only elements that actually exist in the current DOM
            const validSteps = registrySteps.filter(s => {
                try {
                    return !!document.querySelector(s.element);
                } catch (e) {
                    return false;
                }
            });

            if (validSteps.length > 0) {
                return validSteps;
            }
        }

        // 3. Fallback Layout Steps
        return getFallbackSteps();
    }

    // Initialize Driver.js instance
    let activeDriverInstance = null;

    function markTourAsSeen() {
        try {
            localStorage.setItem('portal_tour_seen', '1');
            localStorage.setItem('portal_tour_global_seen', '1');
            localStorage.setItem('portal_tour_completed', '1');
            sessionStorage.setItem('portal_tour_session_seen', '1');
            document.cookie = 'portal_tour_seen=1; path=/; max-age=31536000; SameSite=Lax';
        } catch (e) {}
    }

    function isTourAlreadySeen() {
        try {
            if (localStorage.getItem('portal_tour_seen') === '1' ||
                localStorage.getItem('portal_tour_global_seen') === '1' ||
                localStorage.getItem('portal_tour_completed') === '1' ||
                sessionStorage.getItem('portal_tour_session_seen') === '1' ||
                document.cookie.indexOf('portal_tour_seen=1') !== -1) {
                return true;
            }
        } catch (e) {}
        return false;
    }

    function getDriverInstance() {
        if (!window.driver || !window.driver.js || !window.driver.js.driver) {
            console.warn('Driver.js is not loaded.');
            return null;
        }

        return window.driver.js.driver({
            showProgress: true,
            animate: true,
            popoverClass: 'dreamscore-tour-popover',
            nextBtnText: 'Next &rarr;',
            prevBtnText: '&larr; Back',
            doneBtnText: 'Got It!',
            onDestroyStarted: () => {
                markTourAsSeen();
                activeDriverInstance?.destroy();
            },
            onCloseClick: () => {
                markTourAsSeen();
                activeDriverInstance?.destroy();
            }
        });
    }

    // Public method to trigger the tour
    window.startPortalTour = function (force = false) {
        const pageKey = getPageKey();

        // If not manually triggered by clicking Help/Tour button:
        if (!force) {
            // 1. Only EVER auto-launch on the main Dashboard (/admin or /admin/dashboard), never on subpages
            const isMainDashboard = (pageKey === '/admin' || pageKey === '/admin/dashboard' || pageKey === '/');
            if (!isMainDashboard) {
                return;
            }

            // 2. Check if already seen once globally across the entire browser
            if (isTourAlreadySeen()) {
                return;
            }
        }

        const steps = resolvePageSteps();
        if (!steps || steps.length === 0) {
            return;
        }

        const driver = getDriverInstance();
        if (!driver) return;

        // Mark as seen immediately so it will never repeat again in this browser
        markTourAsSeen();

        activeDriverInstance = driver;
        driver.setSteps(steps);
        driver.drive();
    };

    // Public helper to reset all tours for testing/troubleshooting
    window.resetAllPortalTours = function () {
        try {
            localStorage.removeItem('portal_tour_seen');
            localStorage.removeItem('portal_tour_global_seen');
            localStorage.removeItem('portal_tour_completed');
            sessionStorage.removeItem('portal_tour_session_seen');
            document.cookie = 'portal_tour_seen=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT';
            const keys = Object.keys(localStorage);
            keys.forEach(k => {
                if (k.startsWith('portal_tour_')) {
                    localStorage.removeItem(k);
                }
            });
        } catch (e) {}
        console.log('Portal tours reset.');
        window.startPortalTour(true);
    };

    // Auto-launch only once on initial platform onboarding
    document.addEventListener('DOMContentLoaded', function () {
        // Run check after layout stabilizes
        setTimeout(function () {
            window.startPortalTour(false);
        }, 800);
    });

})();
