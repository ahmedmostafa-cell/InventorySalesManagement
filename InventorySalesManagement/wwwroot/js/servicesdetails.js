$(document).ready(function () {
    $.ajax({
        url: '/Services/GetServiceDetails/' + serviceId,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            var html = `<dl class="row">
                                <dt class="col-sm-2">القسم الرئيسي</dt>
                                <dd class="col-sm-10">${data.mainSectionTitle}</dd>

                                <dt class="col-sm-2">العنوان بالعربية</dt>
                                <dd class="col-sm-10">${data.titleAr}</dd>

                                <dt class="col-sm-2">العنوان بالإنجليزية</dt>
                                <dd class="col-sm-10">${data.titleEn}</dd>

                                <dt class="col-sm-2">الوصف</dt>
                                <dd class="col-sm-10">${data.description}</dd>

                                <dt class="col-sm-2">السعر</dt>
                                <dd class="col-sm-10">${data.price}</dd>

                                <dt class="col-sm-2">الكمية</dt>
                                <dd class="col-sm-10">${data.qty}</dd>

                                <dt class="col-sm-2">نوع المنتج</dt>
                                <dd class="col-sm-10">${data.productType == "Stored" ? "مخزني" : "خدمي"}</dd>
                            </dl>`;

            $("#serviceDetails").html(html);
            $("#editBtn").attr("href", "/Services/Edit/" + data.id);
        },
        error: function () {
            $("#serviceDetails").html('<p class="text-danger text-center">فشل تحميل البيانات.</p>');
        }
    });
});