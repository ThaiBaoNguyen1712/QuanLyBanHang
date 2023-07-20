$.noConflict();
jQuery(document).ready(function ($) {
    $('#myTable').DataTable({
        language: {
            search: "Tìm kiếm:",
            searchPlaceholder: "Nhập từ khóa...",
            lengthMenu: "Hiển thị _MENU_ kết quả",
            info: "Hiển thị _START_ đến _END_ của tổng cộng _TOTAL_ kết quả",
            infoEmpty: "Hiển thị 0 đến 0 của tổng cộng 0 kết quả",
            infoFiltered: "(được lọc từ tổng cộng _MAX_ kết quả)",
            paginate: {
                first: "Đầu",
                last: "Cuối",
                next: "Tiếp",
                previous: "Trước"
            }
        }
    });

    $('#myTable2').DataTable({
        language: {
            search: "Tìm kiếm:",
            searchPlaceholder: "Nhập từ khóa...",
            lengthMenu: "Hiển thị _MENU_ kết quả",
            info: "Hiển thị _START_ đến _END_ của tổng cộng _TOTAL_ kết quả",
            infoEmpty: "Hiển thị 0 đến 0 của tổng cộng 0 kết quả",
            infoFiltered: "(được lọc từ tổng cộng _MAX_ kết quả)",
            paginate: {
                first: "Đầu",
                last: "Cuối",
                next: "Tiếp",
                previous: "Trước"
            }
        }
    });

    $('#myTable-short').DataTable({
        "lengthMenu": [4], // Chỉ hiển thị 4 dòng dữ liệu
        language: {
            search: "Tìm kiếm:",
            searchPlaceholder: "Nhập từ khóa...",
            lengthMenu: "Hiển thị _MENU_ kết quả",
            info: "Hiển thị _START_ đến _END_ của tổng cộng _TOTAL_ kết quả",
            infoEmpty: "Hiển thị 0 đến 0 của tổng cộng 0 kết quả",
            infoFiltered: "(được lọc từ tổng cộng _MAX_ kết quả)",
            paginate: {
                first: "Đầu",
                last: "Cuối",
                next: "Tiếp",
                previous: "Trước"
            }
        }
    });
});
