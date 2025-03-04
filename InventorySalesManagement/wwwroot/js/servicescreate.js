﻿
$(document).ready(function () {

    $.ajax({
        url: getMainSectionsUrl, // Replace with actual controller name
        type: "GET",
        dataType: "json",
        success: function (data) {
            var $dropdown = $("#MainSectionId");
            $dropdown.empty();
            $dropdown.append('<option value="">اختر القسم الرئيسي</option>');

            $.each(data, function (index, section) {
                $dropdown.append('<option value="' + section.id + '">' + section.titleAr + '</option>');
            });
        },
        error: function () {
            console.error("Error loading main sections.");
        }
    });
    $("#createServiceForm").submit(function (event) {
        event.preventDefault(); // Prevent default form submission
        var isValid = true; // Validation flag
        var firstErrorElement = null; // Store first invalid field for scrolling

        // Reset validation messages
        $(".text-danger").text("");

        // Validate required fields
        $(".required-field").each(function () {
            if ($(this).val().trim() === "") {
                var fieldId = $(this).attr("id") + "Validation";
                $("#" + fieldId).text("هذا الحقل مطلوب");
                isValid = false;
                if (!firstErrorElement) firstErrorElement = $(this);
            }
        });

        // Validate MainSectionId
        var mainSectionId = $("#MainSectionId").val();
        if (mainSectionId == "0" || mainSectionId == "") {
            $("#MainSectionIdValidation").text("يرجى اختيار القسم الرئيسي");
            isValid = false;
            if (!firstErrorElement) firstErrorElement = $("#MainSectionId");
        }

        // Validate Price > 0
        var price = parseFloat($("#Price").val());
        if (isNaN(price) || price <= 0) {
            $("#PriceValidation").text("يجب أن يكون السعر أكبر من 0");
            isValid = false;
            if (!firstErrorElement) firstErrorElement = $("#Price");
        }

        // Validate Quantity > 0
        var qty = parseFloat($("#Qty").val());
        if (isNaN(qty) || qty <= 0) {
            $("#QtyValidation").text("يجب أن تكون الكمية أكبر من 0");
            isValid = false;
            if (!firstErrorElement) firstErrorElement = $("#Qty");
        }

        // Validate ProductType (radio button)
        if (!$("input[name='ProductType']:checked").val()) {
            $("#ProductTypeValidation").text("يرجى اختيار نوع المنتج");
            isValid = false;
            if (!firstErrorElement) firstErrorElement = $("#StoredProduct"); // Scroll to first radio button
        }

        // If any field is invalid, scroll to first error and stop submission
        if (!isValid) {
            $("html, body").animate({
                scrollTop: firstErrorElement.offset().top - 50
            }, 500);
            return false;
        }

        // If validation passes, submit the form via AJAX
        var formData = new FormData(this);
        console.log("Form Submitted!");

        $.ajax({
            url: createServiceUrl, // Change to your actual controller
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.success) {
                    $('#successMessage').show().text(response.message);
                    $('#errorMessage').hide();

                    // Scroll to success message
                    $("html, body").animate({
                        scrollTop: $("#successMessage").offset().top - 50
                    }, 500);

                } else {
                    $('#successMessage').hide();
                    if (response.errors) {
                        $('#errorMessage').show().html(response.errors.join("<br>"));
                    } else {
                        $('#errorMessage').show().text(response.message);
                    }

                    // Scroll to error message
                    $("html, body").animate({
                        scrollTop: $("#errorMessage").offset().top - 50
                    }, 500);
                }
            },
            error: function () {
                $('#errorMessage').show().text("حدث خطأ أثناء تنفيذ الطلب.");
                $('#successMessage').hide();

                // Scroll to error message
                $("html, body").animate({
                    scrollTop: $("#errorMessage").offset().top - 50
                }, 500);
            }
        });
    });
});
