$(document).ready(function () {
    $('.mdb-select').material_select();
    $('.select-wrapper.mdb-select').find('li').prop('style', 'text-align:right');
});
$('.submitbuttonRegister').click(function (e) {
    $('#validateError').text("");
    var button = $(this);
    e.preventDefault();
    var errormessage;
    $(this).parent('div').parent('.Form').find('div .requiredInputs').each(function () {

        if ($(this).val()) {
            if ($(this).hasClass('isEmail')) {
                var email = $(this).val();
                $(function () {
                    $.ajax({
                        url: "/UserAccount/validateEmail",
                        type: "POST",
                        data: { email: email },
                        success: (function (data) {
                            if (data[0] == false) {
                                $('#validateError').append('<br/>آدرس پست الکترونیکی اشتباه است لطفا در وارد کردن اطلاعات دقت کنید');
                                $('.isEmail').addClass('invalid');
                                e.preventDefault();
                            }
                            else if (data[1] == false) {
                                $('#validateError').append('<br/>آدرس پست الکترونیکی تکراری است');
                                $('.isEmail').addClass('invalid');
                                e.preventDefault();
                            }
                            if (data[0] == true && data[1] == true) {
                                $('.isEmail').removeClass('invalid');
                            }

                        }),
                        error: (function (xhr) {
                            $('.isEmail').addClass('invalid');

                            e.preventDefault();
                        }),
                    });
                });
            }
            else if ($(this).hasClass('isUsername')) {
                var username = $(this).val();
                $(function () {
                    $.ajax({
                        url: "/UserAccount/validateUsername",
                        type: "POST",
                        data: { username: username },
                        success: (function (data) {
                            if (data[0] == false) {
                                $('#validateError').append('<br/>نام کاربری در سیستم وجود دارد لطفا نام کاربری دیگری انتخاب کنید ');
                                $('.isUsername').addClass('invalid');
                                e.preventDefault();
                            }
                            if (data[0] == true) {
                                $('.isUsername').removeClass('invalid');
                            }

                        }),
                        error: (function (xhr) {
                            $('.isUsername').addClass('invalid');
                            e.preventDefault();
                        }),
                    });
                });
            }
            else {
                $(this).removeClass('invalid');
            }
        }
        else {
            $(this).addClass('invalid');
            e.preventDefault();
        }
    });



    $(document).ajaxStop(function () {

        var inputcount = $(button).parent('div').parent('.Form').find('div.md-form .requiredInputs').length;
        var flag = true;
        for (inputcount; inputcount >= 0; inputcount--) {
            var obj = $(button).parent('div').parent('.Form').find('div input.requiredInputs')[inputcount - 1];
            if (!$(obj).hasClass('invalid')) {
            }
            else {
                flag = false;
                break;
            }
        }
        var select = $(button).parent('div').parent('.Form').find('.mdb-select .requiredInputs');
        var f = true;
        if ($(select).val() == null) {

            var emsg = $('#validateError').text();
            if (!emsg.includes("لطفا نوع کاربری را تعیین کنید")) {
                $('#validateError').append("<br/> لطفا نوع کاربری را تعیین کنید");
                flag = false;
            }
            f = false;
        }

        else {

        }
        if (flag == true && f == true) {
            $(button).parent('div').parent('.Form').submit();
        }
    });

});

$('.submitbuttonLogin').click(function (e) {
    $('#submitError').text("");
    var button = $(this);
    e.preventDefault();

    $(this).parent('div').parent('.Form').find('div .requiredInputs').each(function () {
        if ($(this).val()) {
            $(this).removeClass('invalid');
        }
        else {
            $(this).addClass('invalid');
            e.preventDefault();
        }
    });

    var inputcount = $(button).parent('div').parent('.Form').find('div.md-form .requiredInputs').length;
    var flag = true;
    for (inputcount; inputcount >= 0; inputcount--) {
        var obj = $(button).parent('div').parent('.Form').find('div input.requiredInputs')[inputcount - 1];
        if (!$(obj).hasClass('invalid')) {
        }
        else {
            flag = false;
            break;
        }
    }

    if (flag == true) {
        var username = $('#txt_login_username').val();
        var password = $('#txt_login_password').val();
        $(function () {
            $.ajax({
                url: "/UserAccount/Logincheck",
                type: "POST",
                data: { username, password },
                        success: (function (data) {
                    if (data == false) {
                        $('#submitError').append('<br/> نام کاربری و رمز عبور در سیستم وجود ندارد ');
                        $('#txt_login_username').addClass('invalid');
                        $('#txt_login_password').addClass('invalid');
                        e.preventDefault();
                    }
                    else {
                        $('#txt_login_username').removeClass('invalid');
                        $('#txt_login_password').removeClass('invalid');
                        $(button).parent('div').parent('.Form').submit();
                    }

                        }),
                error: (function (xhr) {
                    $('#txt_login_username').addClass('invalid');
                    $('#txt_login_password').addClass('invalid');
                    e.preventDefault();
                }),
            });
        });
    }
});



$('#gotoregister').click(function () {
    $('#modal-login').modal('hide');
    $('#modal-register').modal('show');
});

$('#gotologin').click(function () {
    $('#modal-register').modal('hide');
    $('#modal-login').modal('show');
});
$('.collapse.navbar-toggleable-xs .nav.navbar-nav .nav-item.dropdown').hover(function () {
    $('.collapse.navbar-toggleable-xs .nav.navbar-nav .nav-item.dropdown').each(function () {
        $(this).removeClass('open');
    });
    $(this).addClass('open');
});