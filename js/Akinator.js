$(document).ready(function () {
    session = new QiSession();

    $('#page_start').show();
    $('#page_selection').hide();

    $('#page_selection_oui').hide();
    $('#page_selection_probablement').hide();
    $('#page_selection_idk').hide();
    $('#page_selection_probablement_non').hide();
    $('#page_selection_non').hide();

    function raise(event, value) {
        session.service("ALMemory").done(function(ALMemory) {
            ALMemory.raiseEvent(event, value);
        });
    }

	$('#page_start').on('click', function() {
        console.log("click start");
        raise('AkinatorDialog/answer', "start")
    });

    $('#page_selection_oui').on('click', function() {
        console.log("click oui");
        raise('AkinatorDialog/oui', "oui")
    });

    $('#page_selection_probablement').on('click', function() {
        console.log("click probablement");
        raise('AkinatorDialog/probablement', "probablement")
    });

    $('#page_selection_idk').on('click', function() {
        console.log("click je ne sais pas");
        raise('AkinatorDialog/idk', "idk")
    });

    $('#page_selection_probablement_pas').on('click', function() {
        console.log("click probablement pas");
        raise('AkinatorDialog/probablement_pas', "probablement_pas")
    });

    $('#page_selection_non').on('click', function() {
        console.log("click Non");
        raise('AkinatorDialog/Non', "non")
    });

});
