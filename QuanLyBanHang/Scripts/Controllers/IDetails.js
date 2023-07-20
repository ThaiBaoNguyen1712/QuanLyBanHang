var Cart = {
    init: function () {
        Cart.regEvents();
    },
    regEvents: function () {

        $('#btnPayment').off('click').on('click', function () {
            window.location.href = "/Admin/BanHang/Payment";
        });

        $('#btnUpdate').off('click').on('click', function () {
            var listProduct = $('.txtQuantity');
            var CartList = [];
            $.each(listProduct, function (i, item) {
                CartList.push({
                    Quantity: $(item).val(),
                    Product: {
                        ID: $(item).data('id')
                    }
                });
            });

            $.ajax({
                url: '/Admin/BanHang/Update',
                data: { CartModel: JSON.stringify(CartList) },
                dataType: 'json',
                type: 'POST',
                success: function (res) {
                    if (res.status == true) {
                        window.location.href = window.location.pathname;;
                    }
                }
            })
        });


        $('#btnDeleteAll').off('click').on('click', function () {


            $.ajax({
                url: '/Admin/BanHang/DeleteAll',
                dataType: 'json',
                type: 'POST',
                success: function (res) {
                    if (res.status == true) {
                        window.location.href = "/Admin/BanHang/Index";
                    }
                }
            })
        });


        $('.btn-delete').off('click').on('click', function (e) {
            e.preventDefault();
            $.ajax({
                data: { id: $(this).data('id') },
                url: '/Admin/BanHang/Delete',
                dataType: 'json',
                type: 'POST',
                success: function (res) {
                    if (res.status == true) {
                        window.location.href = "/Admin/BanHang/Index";
                    }
                }
            })
        });
    }

}
Cart.init();