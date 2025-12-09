// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
import $ from 'jquery';
window.$ = window.jQuery = $;

import { UserAutocomplete } from './js/userAutocomplete.js'
import { initSearchToggle } from './js/searchToggle.js'
import * as GOVUKFrontend from "govuk-frontend";
import * as MOJFrontend from "@ministryofjustice/frontend";

// Expose to window for compatibility
window.GOVUKFrontend = GOVUKFrontend;
window.MOJFrontend = MOJFrontend;

// Initialize everything when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    // Initialize GOV.UK Frontend
    GOVUKFrontend.initAll();
    
    // Initialize MOJ Frontend
    MOJFrontend.initAll();
    
    // Initialize search toggle
    initSearchToggle();
    
    //Temporary solution until the TagHelper Library provides a Date component that only accepts a month and year
    function setAndDisableSignificantDateDayField() {
        const significantDateProp = $("#SignificantDate\\.Day");
        if (significantDateProp.length) {
            significantDateProp.val(1);
            significantDateProp.closest(".govuk-date-input__item").hide();
        }
    }
    
    setAndDisableSignificantDateDayField();
    
    // check for the appropriate element and progressively enhance to a autocomplete when found
    const assignToTarget = document.getElementById('assignment-form-group');
    if (assignToTarget) {
        const autocomplete = new UserAutocomplete();
        autocomplete.init(
            assignToTarget.id,
            'email',
            'Assign to'
        );
    }
    
    const addedByTarget = document.getElementById('added-by-form-group');
    if (addedByTarget) {
        const autocomplete = new UserAutocomplete();
        autocomplete.init(
            addedByTarget.id,
            'email',
            'Added by'
        );
    }
    
    // Cookie banner accept/reject handlers
    const cookieAcceptBtn = document.querySelector('[data-testid="cookie-banner-accept"]');
    if (cookieAcceptBtn) {
        cookieAcceptBtn.addEventListener('click', function() {
            const url = this.getAttribute('data-accept-url');
            if (url) {
                window.location.href = url;
            }
        });
    }
    
    const cookieRejectBtn = document.querySelector('[data-testid="cookie-banner-reject"]');
    if (cookieRejectBtn) {
        cookieRejectBtn.addEventListener('click', function() {
            const url = this.getAttribute('data-reject-url');
            if (url) {
                window.location.href = url;
            }
        });
    }
});
