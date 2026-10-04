/**
 * ExwhyzeeSP Dreams Core - Unified Toast Notification Engine
 * High-performance, lightweight, stackable, responsive Toast Controller
 */

(function () {
    'use strict';

    let toastContainer = null;

    function getToastContainer() {
        if (!toastContainer || !document.body.contains(toastContainer)) {
            toastContainer = document.querySelector('.toast-container');
            if (!toastContainer) {
                toastContainer = document.createElement('div');
                toastContainer.className = 'toast-container';
                toastContainer.setAttribute('aria-live', 'polite');
                document.body.appendChild(toastContainer);
            }
        }
        return toastContainer;
    }

    const defaultIcons = {
        success: 'icon-check-circle',
        warning: 'icon-alert-triangle',
        danger: 'icon-alert-circle',
        error: 'icon-alert-circle',
        info: 'icon-info'
    };

    const defaultTitles = {
        success: 'Success',
        warning: 'Notice',
        danger: 'Action Failed',
        error: 'Error',
        info: 'Information'
    };

    /**
     * Show a modern toast notification
     * @param {Object} options
     * @param {'success'|'warning'|'danger'|'error'|'info'} [options.type='info']
     * @param {string} [options.title]
     * @param {string} options.message
     * @param {number} [options.duration=4500] (in ms, 0 = persistent)
     * @param {string} [options.icon]
     */
    function showToast(options) {
        if (typeof options === 'string') {
            options = { message: options, type: 'info' };
        }

        const type = (options.type || 'info').toLowerCase();
        const title = options.title || defaultTitles[type] || 'Notification';
        const message = options.message || '';
        const duration = options.duration !== undefined ? options.duration : 4500;
        const iconClass = options.icon || defaultIcons[type] || 'icon-info';

        const container = getToastContainer();

        const toast = document.createElement('div');
        toast.className = `dreamscore-toast toast-${type}`;
        toast.setAttribute('role', 'alert');

        toast.innerHTML = `
            <div class="toast-icon-wrap">
                <i class="${iconClass}"></i>
            </div>
            <div class="toast-content">
                <div class="toast-title">${escapeHtml(title)}</div>
                <div class="toast-message">${escapeHtml(message)}</div>
            </div>
            <button type="button" class="toast-close-btn" aria-label="Dismiss notification">
                <i class="icon-x"></i>
            </button>
            ${duration > 0 ? `
            <div class="toast-progress-track">
                <div class="toast-progress-bar"></div>
            </div>` : ''}
        `;

        container.appendChild(toast);

        const closeBtn = toast.querySelector('.toast-close-btn');
        const progressBar = toast.querySelector('.toast-progress-bar');

        let timeoutId = null;
        let startTime = Date.now();
        let remainingTime = duration;
        let isPaused = false;

        function closeToast() {
            if (toast.classList.contains('toast-hiding')) return;
            toast.classList.add('toast-hiding');
            if (timeoutId) clearTimeout(timeoutId);
            setTimeout(() => {
                toast.remove();
            }, 300);
        }

        closeBtn?.addEventListener('click', (e) => {
            e.stopPropagation();
            closeToast();
        });

        if (duration > 0) {
            function startTimer(time) {
                startTime = Date.now();
                remainingTime = time;

                if (progressBar) {
                    progressBar.style.transition = `transform ${remainingTime}ms linear`;
                    progressBar.style.transform = 'scaleX(0)';
                }

                timeoutId = setTimeout(() => {
                    closeToast();
                }, remainingTime);
            }

            function pauseTimer() {
                if (isPaused) return;
                isPaused = true;
                clearTimeout(timeoutId);
                const elapsed = Date.now() - startTime;
                remainingTime = Math.max(0, remainingTime - elapsed);

                if (progressBar) {
                    const currentScale = remainingTime / duration;
                    progressBar.style.transition = 'none';
                    progressBar.style.transform = `scaleX(${currentScale})`;
                }
            }

            function resumeTimer() {
                if (!isPaused) return;
                isPaused = false;
                startTimer(remainingTime);
            }

            toast.addEventListener('mouseenter', pauseTimer);
            toast.addEventListener('mouseleave', resumeTimer);

            // Start countdown
            requestAnimationFrame(() => {
                startTimer(duration);
            });
        }

        return {
            close: closeToast,
            element: toast
        };
    }

    function escapeHtml(text) {
        if (!text) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Global Public Shortcut APIs
    window.showToast = showToast;
    window.toastSuccess = (title, message, duration = 4500) => showToast({ type: 'success', title, message, duration });
    window.toastWarning = (title, message, duration = 5000) => showToast({ type: 'warning', title, message, duration });
    window.toastDanger = (title, message, duration = 6000) => showToast({ type: 'danger', title, message, duration });
    window.toastError = window.toastDanger;
    window.toastInfo = (title, message, duration = 4000) => showToast({ type: 'info', title, message, duration });

    // Automatically consume server-rendered ASP.NET Core TempData alerts
    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('.server-toast-item').forEach(el => {
            const type = el.getAttribute('data-type') || 'info';
            const title = el.getAttribute('data-title') || defaultTitles[type];
            const message = el.getAttribute('data-message') || el.textContent.trim();
            if (message) {
                showToast({ type, title, message });
            }
            el.remove();
        });
    });

})();
