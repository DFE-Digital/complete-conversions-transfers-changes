// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

import { initAll } from "govuk-frontend";
initAll();
import MOJFrontend from "@ministryofjustice/frontend/moj/all";
MOJFrontend.initAll();


//Temporary solution until the TagHelper Library provides a Date component that only accepts a month and year
function setAndDisableSignificantDateDayField() {
    
    var significantDateProp = $("#SignificantDate\\.Day");

    significantDateProp.val(1);
    significantDateProp.closest(".govuk-date-input__item").hide();
}

setAndDisableSignificantDateDayField();
