var input;
$(".Bookman").focus(function(){
    input = $(this);
});

var options = {

    url: function(phrase) {

        return "/UserAccount/GetBookMan";
    },

    getValue: function(element) {
                
        return element.Fname + " " + element.Lname;
    },


    ajaxSettings: {
        type: "POST",
        data: {  },
    },

    preparePostData: function(data) {
        data.phrase = input.val();
        return data;
    }
};

$(".Bookman").easyAutocomplete(options);
        
var publications = {

    url: function(phrase) {

        return "/UserAccount/GetPublications";
    },

    getValue: function(element) {
                
        return element;
    },


    ajaxSettings: {
        type: "POST",
        data: {  },
    },

    preparePostData: function(data) {
        data.phrase = $("#txt_publication").val();
        return data;
    }
};

$('#txt_publication').easyAutocomplete(publications);

$(document).ready(function () {
    $('.md-pills .nav-item a.active').parent('li').prop('style', 'box-shadow: none; background-color: rgba(51,178,187,.2); border-right: 4px solid #33b2bb; color: #212121; width:250px');
});

$('#collapseAge').click(function (e) {
    if ($(this).attr('aria-expanded') == 'false') {
        $('#select_ageCategory').attr('disabled','true');
        $('#AgeCategorySelect .mdb-select input').attr('disabled','true');
        $(this).html("بستن گروه سنی جدید <i class='fa fa-angle-double-right'></i>" );
        $('#txt_agecategoryName').addClass('requiredInputs');
        $('#select_ageCategory').removeClass('requiredInputs');
    }
    else {
        $('#select_ageCategory').removeAttr('disabled');
        $('#AgeCategorySelect .mdb-select input').removeAttr('disabled');
        $(this).html("افزودن گروه سنی جدید  <i class='fa fa-angle-double-left'></i>" );
        $('#txt_agecategoryName').removeClass('invalid');
        $('#txt_agecategoryName').removeClass('requiredInputs');
        $('#select_ageCategory').addClass('requiredInputs');
    }
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

    if(flag == true)
    {
        $(button).parent('div').parent('.Form').submit();
    }
});


$('.collapsewriter').click(function (e) {
    if ($(this).attr('aria-expanded') == 'false') {
        $(this).html("حذف نویسنده <i class='fa fa-angle-double-right'></i>");
    }
    else {
        $(this).html("افزودن نویسنده <i class='fa fa-angle-double-left'></i>");            
    }
});
        
$('.collapsetranslator').click(function (e) {
    if ($(this).attr('aria-expanded') == 'false') {
        $(this).html("حذف مترجم  <i class='fa fa-angle-double-right'></i>");
    }
    else {
        $(this).html("افزودن مترجم <i class='fa fa-angle-double-left'></i>");
    }
});

$('#submitbuttonAddSellerBook').click(function (e) {
    $('#validateError').text("");
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

    var countNotvalid = 0;
            
            
    var txtarea = $(button).parent('div').parent('.Form').find('textarea.requiredInputs').length;
    var inputs = $(button).parent('div').parent('.Form').find('input.requiredInputs').length;
    for(var i = 0 ; i<inputs ; i++)
    {
        var inputvalue =  $(button).parent('div').parent('.Form').find('input.requiredInputs')[i];
        if(inputvalue.value == "")
        {
            countNotvalid = countNotvalid +1;
        }
    }
    for (var i = 0 ; i < txtarea ; i++) {
        var txtvalue = $(button).parent('div').parent('.Form').find('textarea.requiredInputs')[i];
        if (txtvalue.value == "") {
            countNotvalid = countNotvalid + 1;
        }
    }
    if(countNotvalid == 0 )
    {  
                
        var flag= false;
        var errormessage = "";
        var selectage = $(button).parent('div').parent('.Form').find('.mdb-select.requiredInputs#select_ageCategory');
        if ($(selectage).val() == null && selectage.length !=0) {
            errormessage += "لطفا گروه سنی کتاب را انتخاب کنید <br/>";
            flag = false;
        }
        var selecttags = $(button).parent('div').parent('.Form').find('.mdb-select.requiredInputs#select_tags');
        if ($(selecttags).val() == null) {
            errormessage += "لطفا دسته بندی کتاب را انتخاب کنید";
            flag = false;
        } 
        $('#validateError').html(errormessage);
        if($(selectage).val() !=null && $(selecttags).val()!=null)
        {
            flag = true; 
            $(button).parent('div').parent('.Form').submit();
        }
    }
    else{
    }
});