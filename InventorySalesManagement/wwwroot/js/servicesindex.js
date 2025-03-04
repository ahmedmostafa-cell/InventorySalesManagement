$(document).ready(function () {
    $('#servicesTable').DataTable({
        "processing": true,
        "serverSide": false,
        "ajax": {
            "url": "/Services/GetServices",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "titleAr" },
            { "data": "mainSection" },
            { "data": "qty" },
            { "data": "productType" },
            {
                "data": "id",
                "render": function (data, type, row) {
                    return `
                                            <div class="item-action dropdown">
                                                <a href="javascript:void(0)" data-bs-toggle="dropdown" class="icon">
                                                    <i class="fe fe-more-vertical fs-20 text-dark"></i>
                                                </a>
                                                <div class="dropdown-menu dropdown-menu-end">
                                                    <a href="/Services/Edit/${data}" class="dropdown-item btn btn-warning">تعديل</a>
                                                    <a href="/Services/Details/${data}" class="dropdown-item btn btn-info">تفاصيل</a>
                                                    <a href="/Services/Delete/${data}" class="btn btn-danger dropdown-item" onclick="return confirm('هل أنت متأكد من الحذف ؟');">حذف</a>
                                                </div>
                                            </div>`;
                }
            }
        ],
        "language": {
            "search": "البحث",
            "lengthMenu": "عرض _MENU_ سجل لكل صفحة",
            "zeroRecords": "لا توجد نتائج مطابقة",
            "info": "عرض _START_ إلى _END_ من _TOTAL_ سجلات",
            "infoEmpty": "لا توجد بيانات متاحة",
            "infoFiltered": "(تصفية من إجمالي _MAX_ سجلات)",
            "paginate": {
                "first": "الأول",
                "last": "الأخير",
                "next": "التالي",
                "previous": "السابق"
            }
        }
    });
});