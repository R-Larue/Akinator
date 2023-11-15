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
        raise('SimpleWeb/start', 1)
    });

    $('#page_selection_1').on('click', function() {
        console.log("click oui");
        raise('SimpleWeb/oui', 1)
    });

    $('#page_selection_2').on('click', function() {
        console.log("click probablement");
        raise('SimpleWeb/probablement', 1)
    });

    $('#page_selection_3').on('click', function() {
        console.log("click je ne sais pas");
        raise('SimpleWeb/idk', 1)
    });

    $('#page_selection_3').on('click', function() {
        console.log("click probablement pas");
        raise('SimpleWeb/probablement_pas', 1)
    });

    $('#page_selection_3').on('click', function() {
        console.log("click Non");
        raise('SimpleWeb/Non', 1)
    });

});
