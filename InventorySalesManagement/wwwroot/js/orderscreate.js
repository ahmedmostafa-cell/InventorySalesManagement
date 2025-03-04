
$(document).ready(function () {
    let orderItems = [];

    // Update price and total when selecting a service
    $("#serviceDropdown").change(function () {
        let price = parseFloat($("#serviceDropdown option:selected").attr("data-price"));
        $("#servicePrice").val(price);
        let qty = $("#serviceQty").val();
        $("#serviceTotal").val(price * qty);
    });

    // Update total when changing quantity
    $("#serviceQty").on("input", function () {
        let price = parseFloat($("#servicePrice").val());
        let qty = parseInt($(this).val());
        $("#serviceTotal").val(price * qty);
    });

    // Add service to the table
    $("#addServiceBtn").click(function () {
        let serviceId = $("#serviceDropdown").val();
        let serviceName = $("#serviceDropdown option:selected").text();
        let price = parseFloat($("#servicePrice").val());
        let qty = parseInt($("#serviceQty").val());
        let total = price * qty;

        if (!serviceId) {
            alert("يرجى اختيار خدمة");
            return;
        }

        // Check if service already exists
        let existing = orderItems.find(x => x.ServiceId == serviceId);
        if (existing) {
            existing.Quantity += qty;
            existing.Total = existing.Quantity * price;
        } else {
            orderItems.push({ ServiceId: serviceId, ServiceName: serviceName, Price: price, Quantity: qty, Total: total });
        }

        updateTable();
    });

    // Update Table Function
    function updateTable() {
        $("#orderTable tbody").empty();
        let totalInvoice = 0;
        orderItems.forEach((item, index) => {
            totalInvoice += item.Total;

            $("#orderTable tbody").append(`
    <tr>
        <td>${item.ServiceName}</td>
        <td>${item.Price}</td>
        <td><input type="number" class="form-control qtyInput" data-index="${index}" value="${item.Quantity}"></td>
        <td>${item.Total}</td>
        <td><button class="btn btn-danger deleteRow" data-index="${index}">X</button></td>
    </tr>
    `);
        });
        $("#invoiceTotal").val(totalInvoice.toFixed(2)); // Update the invoice total display

    }

    // Update Quantity and Total in Table
    $(document).on("input", ".qtyInput", function () {
        let index = $(this).data("index");
        let newQty = parseInt($(this).val());
        orderItems[index].Quantity = newQty;
        orderItems[index].Total = newQty * orderItems[index].Price;
        updateTable();
    });

    // Delete Row
    $(document).on("click", ".deleteRow", function () {
        let index = $(this).data("index");
        orderItems.splice(index, 1);
        updateTable();
    });

    // Save Order
    $("#saveOrderBtn").click(function () {
        if (orderItems.length === 0) {
            alert("يرجى إضافة خدمات");
            return;
        }

        let orderData = { OrderServices: orderItems };

        $.ajax({
            url: "/Orders/Create",
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify(orderData),
            success: function () {
                alert("تم حفظ الطلب بنجاح!");
                window.location.reload();
            }
        });
    });
});

