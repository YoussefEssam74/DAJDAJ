var dtble;

$(document).ready(function () {
    // Check if status parameter exists in URL
    const urlParams = new URLSearchParams(window.location.search);
    const statusParam = urlParams.get('status');
    if (statusParam) {
        $('#fromStatus').val(statusParam);
    }
    
    loaddata();

    // Change Status Button
    $('#changeStatus').on('click', function () {
        changeStatus();
    });

    // Print New Orders Button (only unprinted)
    $('#printNewOrders').on('click', function () {
        printFiltered('new');
    });

    // Print Buttons
    $('#printToday').on('click', function () {
        printFiltered('today');
    });

    $('#print3Days').on('click', function () {
        printFiltered('3days');
    });

    $('#printWeek').on('click', function () {
        printFiltered('week');
    });

    $('#printAll').on('click', function () {
        printFiltered('all');
    });

    // Export Filtered Button
    $('#exportFiltered').on('click', function () {
        exportFiltered();
    });

    // Reload table when FROM status changes
    $('#fromStatus').on('change', function () {
        dtble.ajax.reload();
    });
});

function loaddata() {
    dtble = $("#mytable").DataTable({
        "responsive": true,
        "ajax": {
            "url": "/Admin/Order/GetData",
            "data": function (d) {
                d.status = $('#fromStatus').val();
            },
            "dataSrc": "data",
            "error": function (xhr, error, thrown) {
                console.error("Error fetching data:", error);
                alert("Failed to load orders data. Please check console for details.");
            }
        },
        "columns": [
            { 
                "data": "id",
                "responsivePriority": 1
            },
            { 
                "data": "name",
                "responsivePriority": 2
            },
            { "data": "phone" },
            { "data": "city" },
            { 
                "data": "orderStatus",
                "responsivePriority": 3
            },
            {
                "data": "totalPrice",
                "render": function (data) {
                    return data ? "EGP " + parseFloat(data).toFixed(2) : "EGP 0.00";
                }
            },
            {
                "data": "id",
                "responsivePriority": 4,
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

function changeStatus() {
    var fromStatus = $('#fromStatus').val();
    var toStatus = $('#toStatus').val();
    
    if (!toStatus) {
        Swal.fire({
            icon: 'warning',
            title: 'No Status Selected',
            text: 'Please select a TO Status.'
        });
        return;
    }

    var message = fromStatus 
        ? `This will change all "${fromStatus}" orders to "${toStatus}"!`
        : `This will change ALL orders to "${toStatus}"!`;

    Swal.fire({
        title: 'Are you sure?',
        text: message,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, change status!'
    }).then((result) => {
        if (result.isConfirmed) {
            // Show loading
            Swal.fire({
                title: 'Updating...',
                text: 'Please wait while we update the orders',
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });

            // AJAX call instead of form submit
            $.ajax({
                url: '/Admin/Order/ChangeAllStatus',
                type: 'POST',
                data: {
                    newStatus: toStatus,
                    status: fromStatus,
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Success!',
                        text: response.message || 'Orders updated successfully',
                        timer: 2000
                    });
                    // Reload table
                    dtble.ajax.reload();
                },
                error: function (xhr) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error!',
                        text: 'Failed to update orders status'
                    });
                }
            });
        }
    });
}

function printFiltered(dateFilter) {
    var fromStatus = $('#fromStatus').val();
    var params = new URLSearchParams();
    
    if (dateFilter === 'new') {
        // Print only unprinted orders - always use Booked status
        params.append('status', 'Booked');
        params.append('onlyUnprinted', 'true');
    } else {
        // For other print options, use the selected status filter
        if (fromStatus) {
            params.append('status', fromStatus);
        }
        
        // Add date filtering
        var today = new Date();
        if (dateFilter === 'today') {
            var todayStr = today.toISOString().split('T')[0];
            params.append('startDate', todayStr);
            params.append('endDate', todayStr);
        } else if (dateFilter === '3days') {
            var threeDaysAgo = new Date(today);
            threeDaysAgo.setDate(today.getDate() - 3);
            params.append('startDate', threeDaysAgo.toISOString().split('T')[0]);
            params.append('endDate', today.toISOString().split('T')[0]);
        } else if (dateFilter === 'week') {
            var weekAgo = new Date(today);
            weekAgo.setDate(today.getDate() - 7);
            params.append('startDate', weekAgo.toISOString().split('T')[0]);
            params.append('endDate', today.toISOString().split('T')[0]);
        }
        // If 'all', don't add date parameters
    }
    
    var queryString = params.toString();
    window.open('/Admin/Order/PrintFiltered' + (queryString ? '?' + queryString : ''), '_blank');
}

function exportFiltered() {
    var fromStatus = $('#fromStatus').val();
    var fromId = $('#fromId').val();
    var toId = $('#toId').val();
    
    // Validate ID range if provided
    if (fromId && toId && parseInt(fromId) > parseInt(toId)) {
        Swal.fire({
            icon: 'warning',
            title: 'Invalid Range',
            text: 'From ID must be less than or equal to To ID.'
        });
        return;
    }
    
    var params = [];
    if (fromStatus) {
        params.push('status=' + encodeURIComponent(fromStatus));
    }
    if (fromId) {
        params.push('fromId=' + encodeURIComponent(fromId));
    }
    if (toId) {
        params.push('toId=' + encodeURIComponent(toId));
    }
    
    var queryString = params.length > 0 ? '?' + params.join('&') : '';
    window.location.href = '/Admin/Order/ExportFilteredToExcel' + queryString;
}
