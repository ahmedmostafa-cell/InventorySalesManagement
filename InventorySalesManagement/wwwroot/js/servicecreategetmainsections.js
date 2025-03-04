$(document).ready(function () {
    $.ajax({
        url: "@Url.Action("GetMainSections", "MainSections")", // Replace with actual controller name
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
});