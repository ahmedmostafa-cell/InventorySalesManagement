
$(document).ready(function () {
    $('#table_id').DataTable({
        paging: true,
        ordering: true,
        "pagingType": "simple_numbers",
        "lengthMenu": [10, 20, 75, 100],
        "oLanguage": {
            "sSearch": "البحث"
        }
    });
});
// تفاصيل الفاتورة
$(document).on("click", ".view-details", function () {
    var invoiceId = $(this).data("id");


    $.ajax({
        url: '/Orders/Details', // Updated to hit the Create endpoint
        type: 'POST',
        contentType: 'application/json', // Ensure the correct content type
        data: JSON.stringify(invoiceId), // Convert JS object to JSON string
        success: function (response) {
            if (response.success) {
                $("#invoiceDetailsBody").empty();
                let totalInvoice = 0;

                $.each(response.orderServices, function (index, item) {
                    let total = item.quantity * item.unitPrice;
                    totalInvoice += total;

                    $("#invoiceDetailsBody").append(`
    <tr>
        <td>${item.serviceName}</td>
        <td>${item.quantity}</td>
        <td>${item.unitPrice.toFixed(2)}</td>
        <td>${total.toFixed(2)}</td>
    </tr>
    `);
                });

                $("#totalInvoiceAmount").text(totalInvoice.toFixed(2));
                $("#invoiceDetailsCard").fadeIn();
            } else {
                alert(response.message);
            }
        },
        error: function (xhr) {
            alert(xhr.responseText || "حدث خطأ أثناء جلب البيانات.");
        }
    });
});
