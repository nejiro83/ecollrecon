﻿function pageLoad(sender, args) {

    $(function () {
        $(".datepicker").datepicker({
            dateFormat: 'MM d, yy'
        });
    });

    $(document).ready(function () {
        var url = window.location.pathname;
        $('.nav-item .nav-link').find('.active').removeClass('active');
        $('ul.nav li').each(function () {
            var hrf = $(this).find('a').attr('href');
            if (hrf.includes(url)) {
                $(this).find('a').addClass('active');
            }
        });
    });
}

function MsgBox(msgTxt, htitle) {
    $(function () {
        $('#msgBox').html(msgTxt);
        $('#msgBox').dialog({
            modal: true,
            width: 'auto',
            resizable: false,
            draggable: false,
            title: htitle,
            close: function (event, ui) { $('body').find('#msgBox').remove(); },
            buttons: {
                'OK': function () { $(this).dialog('close'); }
            }
        })
    }).dialog("open");
}

function reconDetails(rownumber, ucaramount, creditid, reconno, recontype) {

    $('#txtRowNo').val(rownumber);
    $('#txtBankInsti').val($('#ddlBankInsti :selected').text());
    $('#txtReconType').val($('#ddlReconType :selected').text());
    $('#txtUCARAmount').val(ucaramount);

    $('#txtCreditID').val(creditid);
    $('#txtUCARNo').val(reconno);

    switch (recontype) {

        case 'AR':
            $('#reconModal').modal('show');

            break;
        case 'UC':
            location.href = 'ReconPerUC.aspx' +
                '?crid=' + creditid +
                '&ucno=' + reconno;
            return false;
            break;
    }
}

function uncreditedModal(bankinsticode, bankinsti, transdate) {

    $('#txtBankInstiCode').val(bankinsticode);
    $('#txtBankInsti').val(bankinsti);
    $('#txtTransDate').val(transdate);

    $('#creditLineModal').modal('show');
}