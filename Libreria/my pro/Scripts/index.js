
$(document).ready(function () {
    $('#advanceSearch').click(function (e) {
        if ($(this).attr('aria-expanded') == 'false') {
            $('#txt_search').prop('disabled', true);
            $('#txt_search').removeClass('invalid');
            $('#txt_search').val("");
            $('#searchbook').hide();
        }
        else {
            $('#txt_search').prop('disabled', false);
            $('#txt_title , #txt_writer, #txt_translator, #txt_publisher').removeClass('invalid');
            $('#txt_title , #txt_writer, #txt_translator, #txt_publisher').val("");
            $('#searchbook').show();
        }
    });
    new WOW().init();

});
$('#searchbook').click(function (e) {
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
        $(button).parent('div').parent('.Form').submit();
    }
});
$('#advancesearchbook').click(function (e) {
    $('#submitError').text("");
    var button = $(this);
    e.preventDefault();

    $(this).parent('.form-group').parent('.Form').find('div .requiredInputs').each(function () {
        if ($(this).val()) {
            $(this).removeClass('invalid');
        }
        else {
            $(this).addClass('invalid');
            e.preventDefault();
        }
    });

    var inputcount = $(this).parent('.form-group').parent('.Form').find('div .requiredInputs').length;
    var flag = true;
    for (inputcount; inputcount >= 0; inputcount--) {
        var obj = $(this).parent('.form-group').parent('.Form').find('div .requiredInputs')[inputcount - 1];
        if (!$(obj).hasClass('invalid')) {
        }
        else {
            flag = false;
            break;
        }
    }

    if (flag == true) {
        $(this).parent('.form-group').parent('.Form').submit();
    }
});