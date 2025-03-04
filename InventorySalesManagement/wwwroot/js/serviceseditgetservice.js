$(document).ready(function () {
    var savedMainSectionId = null; // Store the selected value for later use

    if (serviceId) {
        // Step 1: Fetch Service Details First
        $.ajax({
            url: "/Services/GetServiceDetails/" + serviceId,
            type: "GET",
            success: function (data) {
                if (data) {
                    console.log(data);
                    $("#Id").val(data.id);
                    $("#TitleAr").val(data.titleAr);
                    $("#TitleEn").val(data.titleEn);
                    $("#Description").val(data.description);
                    $("#Price").val(data.price);
                    $("#Qty").val(data.qty);
                    savedMainSectionId = data.mainSectionTitle; // Store the value
                    $("input[name='ProductType'][value='" + data.productType + "']").prop("checked", true);

                    // Step 2: After retrieving service data, load dropdown
                    loadMainSections(savedMainSectionId);
                }
            },
            error: function () {
                alert("Failed to load service details.");
            }
        });
    } else {
        // If no serviceId, still load dropdown
        loadMainSections(null);
    }

    // Function to load Main Sections
    function loadMainSections(selectedId) {
        $.ajax({
            url: "/MainSections/GetMainSections",
            type: "GET",
            success: function (sections) {
                console.log(sections);
                var dropdown = $("#MainSectionId");
                dropdown.empty();
                dropdown.append('<option value="">اختر القسم الرئيسي</option>');

                $.each(sections, function (index, section) {
                    var isSelected = selectedId && selectedId == section.id ? "selected" : "";
                    dropdown.append(`<option value="${section.id}" ${isSelected}>${section.titleAr}</option>`);
                });

                // Ensure correct selection after options are loaded
                if (selectedId) {
                    dropdown.val(selectedId);
                }
            },
            error: function () {
                alert("Failed to load main sections.");
            }
        });
    }
});