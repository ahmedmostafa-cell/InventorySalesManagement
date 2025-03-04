$(document).ready(function () {
    $('#table_id').DataTable({
        paging: true,
        ordering: true,
        "pagingType": "simple_numbers",
        "lengthMenu": [5, 20, 75, 100],
        "oLanguage": {
            "sSearch": "البحث"
        }
    });
});


