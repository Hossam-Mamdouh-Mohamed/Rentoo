
function ajaxRequest(type, url, data, dataType, async, contentType, processData, cache, showLoader) {
    if (dataType == null) {
        dataType = 'json';
    }
    if (contentType == null) {
        contentType = 'application/x-www-form-urlencoded; charset=UTF-8';
    }
    if (processData == null) {
        processData = true;
    }
    if (cache == null) {
        cache = false;
    }
    if (async == null) {
        async = true;
    }
    if (showLoader == null) {
        showLoader = true;
    }
    var options = {
        dataType: dataType,
        contentType: contentType,
        cache: cache,
        type: type,
        data: data || null,
        processData: processData,
        async: async,
        beforeSend: function () {
            if (showLoader) {
                ShowLoader();
            }
            else {
                HideLoader();
            }
        },
        complete: function () {
            HideLoader();
        }
    };
    var antiForgeryToken = $('#antiForgeryToken').val();
    if (antiForgeryToken) {
        options.headers = {
            'RequestVerificationToken': antiForgeryToken
        };
    }
    return $.ajax(url, options, {
        success: function (data, textStatus, request) {

        },
        complete: function () {
            /*todo: hide loader*/
        },
        error: function () {
            /*todo: hide loader*/
        }
    });
}
function ajaxInsideContentRequest(type, url, data, dataType, async, contentType, processData, cache, showLoader) {
    if (dataType == null) {
        dataType = 'json';
    }
    if (contentType == null) {
        contentType = 'application/x-www-form-urlencoded; charset=UTF-8';
    }
    if (processData == null) {
        processData = true;
    }
    if (cache == null) {
        cache = false;
    }
    if (async == null) {
        async = true;
    }
    if (showLoader == null) {
        showLoader = true;
    }
    var options = {
        dataType: dataType,
        contentType: contentType,
        cache: cache,
        type: type,
        data: data || null,
        processData: processData,
        async: async,
        beforeSend: function () {
            if (showLoader) {
                $('.inside-content-loader').css('display', 'block');
            }
            else {
                $('.inside-content-loader').css('display', 'none');
            }
        },
        complete: function () {
            $('.inside-content-loader').css('display', 'none');
        }
    };
    var antiForgeryToken = $('#antiForgeryToken').val();
    if (antiForgeryToken) {
        options.headers = {
            'RequestVerificationToken': antiForgeryToken
        };
    }
    return $.ajax(url, options, {
        success: function (data, textStatus, request) {

        },
        complete: function () {
            /*todo: hide loader*/
        },
        error: function () {
            /*todo: hide loader*/
        }
    });
}

//#region message alerts
//class: info, error, warning, success
function ViewMsg() {
    if ($('#tempMsg').val() != '') {
        var res = $('#tempMsg').val().split(',');
        switch (res[0]) {
            case 'success': {
                $('#toastTitle').html(res[1]);
                $('#toastContent').html(res[2]);
                $('.toast').addClass('success');
                $('.toast').toast('show');
                break;
            }
            case 'info': {
                $('#toastTitle').html(res[1]);
                $('#toastContent').html(res[2]);
                $('.toast').addClass('info');
                $('.toast').toast('show');
                break;
            }
            default: {
                $('#errorTitle').html(res[1]);
                $('#errorContent').html(res[2]);
                $('#error-modal').modal('show');
                break;
            }
        }
    }
}
function ViewMsgContent(resultMsg) {
    if (resultMsg != null && resultMsg != 'undefined' && resultMsg != '') {
        var res = resultMsg.split(',');
        switch (res[0]) {
            case 'success': {
                $('#toastTitle').html(res[1]);
                $('#toastContent').html(res[2]);
                $('.toast').addClass('success');
                $('.toast').toast('show');
                break;
            }
            case 'info': {
                $('#toastTitle').html(res[1]);
                $('#toastContent').html(res[2]);
                $('.toast').addClass('info');
                $('.toast').toast('show');
                break;
            }
            default: {
                $('#errorTitle').html(res[1]);
                $('#errorContent').html(res[2]);
                $('#error-modal').modal('show');
                break;
            }
        }
    }
}
//#endregion

//#region Loader
function ShowLoader() {
    $('.loader_wrapper').css('display', 'block');
}
function HideLoader() {
    $('.loader_wrapper').css('display', 'none');
}

$('.naamaform').on('submit', function () {
    ShowLoader();
    if ($(this).valid()) {
        var hidLoad = $('#tempShow').val();
        if (hidLoad == '1') {
            HideLoader();
        }
    }
    else {
        HideLoader();
        ViewMsgContent(defaultErrorMessage);
    }
    if (!isEmptyObject($(this).validate()['pending'])) {
        HideLoader();
    }
});
window.addEventListener('DOMContentLoaded', function () {
    ShowLoader();
}, true);
window.onload = function () {
    HideLoader();
};
function isEmptyObject(obj) {
    for (var prop in obj) {
        if (Object.prototype.hasOwnProperty.call(obj, prop)) {
            return false;
        }
    }
    return true;
}
//#endregion

//#region popover ?
function naamaPopover() {
    const popoverTriggerList = document.querySelectorAll('[data-bs-toggle="popover"]');
    const popoverList = [...popoverTriggerList].map(
        (popoverTriggerEl) => new bootstrap.Popover(popoverTriggerEl)
    );
}
//#endregion

//#region Tooltip
function naamaTooltip() {
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    const tooltipList = [...tooltipTriggerList].map(
        (tooltipTriggerEl) => new bootstrap.Tooltip(tooltipTriggerEl)
    );
}
//#endregion

//#region input validations
$('.CustomCalender').on('paste', function (e) {
    e.preventDefault();
});
$('.CustomCalender').on('keypress', function (evt) {
    evt.preventDefault();
});

$('.allowOnlyNumber').attr('inputmode', 'numeric');
$('.allowOnlyNumber').keypress(function (evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        $('#' + evt.currentTarget.parentNode.parentNode.getElementsByClassName('pallon')[0].id).show();
        return false;
    }
    $('#' + evt.currentTarget.parentNode.parentNode.getElementsByClassName('pallon')[0].id).hide();
    return true;
});
$('.allowOnlyEnglish').keypress(function (evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode != 8) {
        if ((charCode < 0x0600 || charCode > 0x06FF)) {
            $('#' + evt.currentTarget.parentNode.parentNode.getElementsByClassName('pallon')[0].id).hide();
            return true;
        }
    }
    $('#' + evt.currentTarget.parentNode.parentNode.getElementsByClassName('pallon')[0].id).show();
    return false;
});
$('.allowOnlyEnglishLetters').keypress(function (evt) {
    reg = /[A-Za-z ]/g;
    if (reg.test(event.key)) {
        $('#' + evt.currentTarget.parentNode.parentNode.getElementsByClassName('pallon')[0].id).hide();
        return true;
    }
    else
    //var charCode = (evt.which) ? evt.which : event.keyCode
    //if (charCode != 8) {
    //    if (charCode == 96 || charCode == 126 || charCode == 34) {
    //        $('#' + evt.currentTarget.parentNode.parentNode.getElementsByClassName('pallon')[0].id).show();
    //        return false;
    //    }
    //    if ((charCode < 0x0600 || charCode > 0x06FF)) {
    //        var reg = /[\d$@.$!%*#?&-_()^+\|/{}<>"]/;
    //        if (!reg.test(event.key)) {
    //            $('#' + evt.currentTarget.parentNode.parentNode.getElementsByClassName('pallon')[0].id).hide();
    //            return true;
    //        }
    //    }
    //}
    {
        $('#' + evt.currentTarget.parentNode.parentNode.getElementsByClassName('pallon')[0].id).show();
        return false;
    }
});
$('.allowOnlyDecimal').keypress(function (evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 46) {
        $('#' + evt.currentTarget.parentNode.parentNode.getElementsByClassName('pallon')[0].id).show();
        return false;
    }
    $('#' + evt.currentTarget.parentNode.parentNode.getElementsByClassName('pallon')[0].id).hide();
    return true;
});
$('.allowOnlyArabic').keypress(function (evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    var englishAlphabetDigits = /[A-Za-z]/g;
    var key = String.fromCharCode(evt.which);
    if (charCode == 8 || charCode == 37 || charCode == 39 || englishAlphabetDigits.test(key)) {
        if (charCode == 32) {
            $('#' + evt.currentTarget.parentNode.parentNode.getElementsByClassName('pallon')[0].id).show();
            return true;
        }
        else {
            $('#' + evt.currentTarget.parentNode.parentNode.getElementsByClassName('pallon')[0].id).show();
            return false;
        }
    }
    $('#' + evt.currentTarget.parentNode.parentNode.getElementsByClassName('pallon')[0].id).hide();
    return true;
});
$('.allowOnlyArabicLetters').keypress(function (evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    var englishAlphabetDigits = /[A-Za-z]/g;
    var reg = /[\d$@.$!%*#?&-_()^+\|/{}<>]/;
    var key = String.fromCharCode(evt.which);
    if (charCode == 8 || charCode == 37 || charCode == 39 || englishAlphabetDigits.test(key) || reg.test(event.key)) {
        if (charCode == 32) {
            $('#' + evt.currentTarget.parentNode.parentNode.getElementsByClassName('pallon')[0].id).show();
            return true;
        }
        else {
            $('#' + evt.currentTarget.parentNode.parentNode.getElementsByClassName('pallon')[0].id).show();
            return false;
        }
    }
    else if (charCode == 96 || charCode == 126 || charCode == 34) {
        $('#' + evt.currentTarget.parentNode.parentNode.getElementsByClassName('pallon')[0].id).show();
        return false;
    }
    $('#' + evt.currentTarget.parentNode.parentNode.getElementsByClassName('pallon')[0].id).hide();
    return true;
});
$('.NotAllowSpace').keypress(function (evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode == 32) {
        $('#' + evt.currentTarget.parentNode.parentNode.getElementsByClassName('pallon')[0].id).show();
        return false;
    }
    $('#' + evt.currentTarget.parentNode.parentNode.getElementsByClassName('pallon')[0].id).hide();
    return true;
});
$('.allowOnlyNumber, .allowOnlyDecimal, .allowOnlyEnglish, .allowOnlyEnglishLetters, .allowOnlyArabic, .allowOnlyArabicLetters, .NotAllowSpace').focusout(function (evt) {
    $('#' + evt.currentTarget.parentNode.parentNode.getElementsByClassName('pallon')[0].id).hide();
});
$('.NonPaste').on('paste', function (evt) {
    evt.preventDefault();
});
$('.NonDrop').on('drop', function (evt) {
    evt.preventDefault();
});
//#endregion


function checkWidth() {
    if ($(window).width() <= 990) {
        $('.main-board-links').addClass('aside-closed');
    } else {
        $('.main-board-links').removeClass('aside-closed');
    }
}