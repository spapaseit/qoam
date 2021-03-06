﻿function initializeForm(culture) {

    Globalize.culture(culture);

    $('span.field-validation-valid, span.field-validation-error').each(function () {
        $(this).addClass('help-inline');
    });

    $('.validation-summary-errors').each(function () {
        $(this).addClass('alert alert-error');
    });

    $('form').submit(function () {
        if ($(this).valid()) {
            $(this).find('div.form-group').each(function () {
                if ($(this).find('span.field-validation-error').length == 0) {
                    $(this).removeClass('has-error');
                }
            });
        }
        else {
            $(this).find('div.form-group').each(function () {
                if ($(this).find('span.field-validation-error').length > 0) {
                    $(this).addClass('has-error');
                }
            });
        }
    });

    $('form').each(function () {
        $(this).find('div.form-group').each(function () {
            if ($(this).find('span.field-validation-error').length > 0) {
                $(this).addClass('has-error');
            }
        });
    });
}

function createTypeahead(selector, remoteUrl) {
    var bloodhoundInstance = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.whitespace('value'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        remote: remoteUrl + '?query=%QUERY'
    });

    bloodhoundInstance.initialize();

    $(selector).typeahead(null, {
        displayKey: 'value',
        source: bloodhoundInstance.ttAdapter()
    });
}

function updateSwotMatrix(selector, input) {
    $($(input).val().split(',')).each(function () {
        if (this.length > 0) {
            $(selector).find('.' + this).addClass('verdict-' + this).addClass('selected');
        }
    });
}

function setupLinkFilters() {
    $("#open-access-filter").on("click", function (e) {
        e.preventDefault();
        var $field = $("#OpenAccess");
        var currentValue = $field.val();

        if (currentValue === "" || currentValue === false || currentValue.toLowerCase() === "false")
            $field.val(true);
        else
            $field.val("");

        $(this).toggleClass("filter-enabled");
        $("#hybrid-filter").removeClass("filter-enabled");

        $("#search-form").submit();
    });

    $("#hybrid-filter").on("click", function (e) {
        e.preventDefault();
        var $field = $("#OpenAccess");
        var currentValue = $field.val();

        if (currentValue === "" || currentValue === true || currentValue.toLowerCase() === "true")
            $field.val(false);
        else
            $field.val("");

        $(this).toggleClass("filter-enabled");
        $("#open-access-filter").removeClass("filter-enabled");

        $("#search-form").submit();
    });

    $("#institutional-discount-filter").on("click", function(e) {
        e.preventDefault();
        var $field = $("#InstitutionalDiscounts");
        var currentValue = $field.val();

        if (!currentValue)
            $field.val(true);
        else
            $field.val("");

        $(this).toggleClass("filter-enabled");
        $("#search-form").submit();
    });
}

function initialLinkFilterState() {
    var openAccess = $("#OpenAccess").val();

    if (openAccess !== undefined && openAccess !== null && openAccess !== "") {
        if (openAccess === true || openAccess.toLowerCase() === "true")
            $("#open-access-filter").addClass("filter-enabled");
        else
            $("#hybrid-filter").addClass("filter-enabled");
    }

    var institutionalDiscounts = $("#InstitutionalDiscounts").val();

    if(institutionalDiscounts)
        $("#institutional-discount-filter").addClass("filter-enabled");
}

function initDisciplinesSelect() {
    $("#SelectedDisciplines").on("chosen:ready", function () {
        $("#loading").hide();
        $("#discipline-container").show();
    }).chosen({
        width: "100%",
        search_contains: true,
        placeholder_text_multiple: "Search by discipline"
    });
}