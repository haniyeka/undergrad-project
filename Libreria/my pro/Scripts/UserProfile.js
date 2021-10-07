$(document).ready(function () {
    $('.md-pills .nav-item a.active').parent('li').prop('style', 'box-shadow: none; background-color: rgba(51,178,187,.2); border-right: 4px solid #33b2bb; color: #212121; width:250px');
});
$('.md-pills .nav-item a').on('click', function () {
    $('.md-pills .nav-item').each(function () {
        $(this).prop('style', '');
    });
    $(this).parent('li').prop('style', 'box-shadow: none; background-color: rgba(51,178,187,.2); border-right: 4px solid #33b2bb; color: #212121; width:250px ');
});


$('.submiteditprofile').click(function (e) {
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
