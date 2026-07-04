var dtble;

$(document).ready(function () {
    loaddata();
});

function loaddata() {
    dtble = $("#mytable").DataTable({
        "ajax": {
            "url": "/Admin/Order/GetData",
            "dataSrc": "data",
            "error": function (xhr, error, thrown) {
                console.error("Error fetching data:", error);
                alert("Failed to load orders data. Please check console for details.");
            }
        },
        "columns": [
            { "data": "id" },
            { "data": "name" },
            { "data": "phone" },
            { "data": "city" },
            { "data": "orderStatus" },
            {
                "data": "totalPrice",
                "render": function (data) {
                    return data ? "EGP " + parseFloat(data).toFixed(2) : "EGP 0.00";
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <a href="/Admin/Order/Details?orderid=${data}" 
                           class="btn btn-primary btn-sm">
                            Details
                        </a>
                    `;
                },
                "orderable": false
            }
        ],
        "language": {
            "emptyTable": "No orders found",
            "zeroRecords": "No matching orders found"
        }
    });
}