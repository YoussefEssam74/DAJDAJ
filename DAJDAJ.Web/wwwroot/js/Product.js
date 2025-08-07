var dtble;

$(document).ready(function () {
    $('#mytable').DataTable({
        "ajax": {
            "url": "/Admin/Product/GetData",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "name" },
            { "data": "size" },
            { "data": "price" },
            { "data": "color" },
            { "data": "category.name" },
            {
                "data": "productVariants",
                "render": function (data, type, row) {
                    if (!data || data.length === 0) return '<span class="text-muted">No variants</span>';
                    let html = '';
                    for (let i = 0; i < data.length; i++) {
                        html += `<div>${data[i].color} - ${data[i].size}: <b>${data[i].quantity}</b></div>`;
                    }
                    return html;
                },
                "orderable": false,
                "searchable": false
            },
            {
                "data": "id",
                "render": function (data, type, row) {
                    return `<a href="/Admin/Product/Edit/${data}" class="btn btn-sm btn-warning">Edit</a> ` +
                        `<a href="/Admin/Product/Delete/${data}" class="btn btn-sm btn-danger">Delete</a>`;
                },
                "orderable": false,
                "searchable": false
            }
        ],
        "language": {
            "emptyTable": "No products found"
        }
    });
});

function DeleteItem(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                url: url,
                type: "Delete",
                success: function (data) {
                    if (data.success) {
                        dtble.ajax.reload();
                        toaster.success(data.message);
                    } else {
                        toaster.error(data.message);
                    }
                }
            });
            Swal.fire({
                title: "Deleted!",
                text: "Your file has been deleted.",
                icon: "success"
            });
        }
    });
}
