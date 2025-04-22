
var naama = {
    _documentReady: function () {
        $(document).ready(function () {
            ViewMsg();
            checkWidth();
            naamaTooltip();
            naamaPopover();
        });
    },
    _confirmMessage: function () {
        $('#search-container').delegate('.btnShowConfirm', 'click', function () {
            let that = this;
            if ($(that).data('title')) {
                $('#confirm-title').html($(that).data('title'));
            }
            if ($(that).data('content')) {
                $('#confirm-content').html($(that).data('content'));
            }
            if ($(that).data('yes')) {
                $('#confirm-yes').text($(that).data('yes'));
            }
            if ($(that).data('no')) {
                $('#confirm-no').text($(that).data('no'));
            }

            $('#confirm-modal').modal('show');

            $('#confirm-modal').on('click', '#confirm-yes', function (e) {
                ajaxRequest('POST', $(that).attr('href')).done(function (result) {
                    $('#confirm-modal').modal('hide');
                    ViewMsgContent(result);
                    if (result.split(',')[0] == 'success') {
                        if ($(that).data('action') == 'refresh') {
                            location.reload();
                        }
                        else {
                            $(that).closest('tr').remove();
                        }
                    }
                });
            });

            return false;
        });
    },
    _showLogoutCofirm: function () {
        $('body').delegate('.btnlogoutcofirm', 'click', function () {
            $('#logoutcofirmModal').modal('show');
        });
    },
    _search: function () {
        $("#SearchCriteria #Search").click(function () {
            var data = $("#SearchForm").serializeArray();
            search(data);
        });
    },
    _searchText: function () {
        $('.SearchSm').keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                var data = $("#SearchForm").serializeArray();
                search(data);
            }
        });
    },

    _dataTable: function () {
        let table = $('.naamatable');
        table = $('.naamatable').DataTable({
            pageLength: 10,
            deferRender: true,
            retrieve: true,
            responsive: true,
            ordering: false,
            layout: {
                topStart: {},
                topEnd: {},
                bottomStart: {},
                bottomEnd: {
                    pageLength: {
                        menu: [5, 10, 25, 50, 100],
                    },
                    paging: {
                        numbers: 3,
                    },
                },
            },
            // language: languageAR,
            language: {
                paginate: {
                    next: '<i class="hgi-stroke hgi-arrow-left-01"></i>',
                    previous: '<i class="hgi-stroke hgi-arrow-right-01"></i>',
                },
                search: 'بحث:',
                lengthMenu: 'يتم عرض _MENU_',
                info: 'عرض _START_ الى _END_ من _TOTAL_ سجل',
                infoEmpty: 'عرض 0 الى 0 من 0 سجل',
                infoFiltered: '(مصفي من _MAX_ سجل)',
                emptyTable: 'لا توجد بيانات متاحة في الجدول',
                zeroRecords: 'لم يتم العثور على اية سجلات مطابقة',
            },
            pagingType: 'simple_numbers',
        });
        $('#search-input-field').on('input', function () {
            table.search($(this).val()).draw();
        });
    },
    _select2: function () {
        $('.select2').select2({
            theme: 'bootstrap-5',
        });
        $('.select2').on('select2:open', function (e) {
            var data = e.params.data;
            // set search placeholder to the select placeholder prop
            var placeholder = $(this).data('search-placeholder');
            $('.select2-search__field').attr('placeholder', placeholder);
            if ($(this).is(':invalid')) {
                // add is-invalid class to the nearest select2 container
                $('.select2-container .select2-dropdown').addClass('is-invalid');
            } else {
                // remove is-invalid class from the nearest select2 container
                $('.select2-container .select2-dropdown').removeClass('is-invalid');
            }
        });
        $('.select2').on('select2:close', function (e) {
            // revalidate the field when it is closed
            $(this).valid();
        });
    }

}


function search(data) {
    ajaxRequest($("#SearchForm").attr('method'), $("#SearchForm").attr('action'), data, 'html').done(function (html) {
        $("#SearchTableContainer").html(html);
    });
}
