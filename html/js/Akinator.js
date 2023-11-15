$(document).ready(function () {
    session = new QiSession();

    $('#page_start').show();
    $('#page_selection').hide();

    function raise(event, value) {
        session.service("ALMemory").done(function(ALMemory) {
            ALMemory.raiseEvent(event, value);
        });
    }

	$('#page_start').on('click', function() {
        console.log("click start");

        raise('AkinatorDialog/answer', "start")
    });

    $('#test').on('click', function() {
        console.log("click oui");
        document.getElementById("test").innerHTML = "test";
        raise('AkinatorDialog/answer', "oui")
    });

    $('#page_selection_probablement').on('click', function() {
        console.log("click probablement");
        raise('AkinatorDialog/answer', "probablement")
    });

    $('#page_selection_idk').on('click', function() {
        console.log("click je ne sais pas");
        raise('AkinatorDialog/answer', "idk")
    });

    $('#page_selection_probablement_pas').on('click', function() {
        console.log("click probablement pas");
        raise('AkinatorDialog/answer', "probablement_pas")
    });

    $('#page_selection_non').on('click', function() {
        console.log("click Non");
        raise('AkinatorDialog/answer', "non")
    });

});

document.getElementById("page_yes").onclick = function() {myFunction()};

function myFunction() {
    // document.getElementById("page_yes").innerHTML = "YOU CLICKED ME!";

    var elem = document.getElementById("page_yes");

    if (elem.innerHTML == "Ceci est une question") {
        elem.innerHTML = "YOU CLICKED ME!";
    } else {
        elem.innerHTML = "Ceci est une question";
    }
}