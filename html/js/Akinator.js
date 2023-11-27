$(document).ready(function () {
    session = new QiSession();

    // $('#page_start').show();
    // $('#page_selection').hide();

    function raise(event, value) {
        session.service("ALMemory").done(function (ALMemory) {
            ALMemory.raiseEvent(event, value);
        });
    }

    function refresh_question() {
        var question_text = document.getElementById("question")

        session.service("ALMemory").then(function (m) {
            m.getData('question').then(function (data) {
                question_text.innerHTML = data;
            });
        });
    }

    $('#page_start').on('click', function () {
        console.log("click start");
        raise('AkinatorDialog/answer', "start");
    });

    $('#page_selection_oui').on('click', function () {
        console.log("click oui");
        raise('AkinatorDialog/answer', "0");
        refresh_question()
    });

    $('#page_selection_probablement').on('click', function () {
        console.log("click probablement");
        raise('AkinatorDialog/answer', "3");
        refresh_question()
    });

    $('#page_selection_idk').on('click', function () {
        console.log("click je ne sais pas");
        raise('AkinatorDialog/answer', "2");
        refresh_question()
    });

    $('#page_selection_probablement_pas').on('click', function () {
        console.log("click probablement pas");
        raise('AkinatorDialog/answer', "4");
        refresh_question()
    });

    $('#page_selection_non').on('click', function () {
        console.log("click Non");
        raise('AkinatorDialog/answer', "1");
        refresh_question()
    });

});
