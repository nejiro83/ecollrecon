function pageLoad(sender, args) {

    var d = new Date();
    var day = d.getDate();
    var month = d.getMonth();
    var year = d.getFullYear();
    var datestring = month + "/" + day + "/" + year;

    const months = ['January', 'February', 'March', 'April',
        'May', 'June', 'July', 'August',
        'September', 'October', 'November', 'December'
    ];

    datestring = months[month] + " " + day + ", " + year;

    $(function () {
        $('.datepicker').on('change blur', function () {

            var isvalid = Date.parse($(this).val());

            if ($(this).val().trim().length === 0 || isNaN(isvalid)) {
                $(this).val(datestring);
            }
        });

        $(".datepicker").datepicker({
            dateFormat: 'MM d, yy',
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

    $('#creditLineModal').on('hidden.bs.modal', function () {

        $('#txtAmountCredited').val(0);
        $('#dtpmodalCreditDate').val(datestring);
        $('#dtpTransDateFrom').val(datestring);
        $('#dtpTransDateTo').val(datestring);

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

function checkExcFields() {

    if ($('#txtTransRefNo').val().length === 0) {
        $('#lblMessage').val('Trans Ref No is blank');
        return false;
    }

    if ($('#txtExcRemarks').val().length === 0) {
        $('#lblMessage').val('Trans Ref No is blank');
        return false;
}

}

function checkCreditLineFields() {

    var creditDate = $('#dtpmodalCreditDate').val();
    var transDateFrom = $('#dtpTransDateFrom').val();
    var transDateTo = $('#dtpTransDateTo').val();
    var amountCredited = $('#txtAmountCredited').val();

    var isValid = Date.parse(creditDate);

    if (isNaN(isValid)) {

        $('#lblMessage').text('Invalid Credit Date');
        return false;

    }

    isValid = Date.parse(transDateFrom);

    if (isNaN(isValid)) {

        $('#lblMessage').text('Invalid Transaction Date From');
        return false;

    }

    isValid = Date.parse(transDateTo);

    if (isNaN(isValid)) {

        $('#lblMessage').text('Invalid Transaction Date To');
        return false;

    }

    if (isNaN(amountCredited)) {

        $('#lblMessage').text('Invalid Amount Credited');
        return false;

    }

    if (Date.parse(transDateFrom) > Date.parse(transDateTo)) {

        $('#lblMessage').text('Invalid placement of transaction dates.');
        return false;

    }
}