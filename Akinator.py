#!/usr/bin/env python
# -*- coding: utf-8 -*-

# run with python 03_...py --qi-url="tcp://ip_robot:9559"

import qi
import requests
import time
import sys
import argparse


class AkinatorModule:
    """
    Wow, there should be some doc here too
    """
    def __init__(self, session):
        """
        """
        print ("Akinator Module init")
        self.hasStarted = False
        self.session = session
        # Animated speech
        self.animated_speech = self.session.service("ALAnimatedSpeech")
        # Init ALMemory service
        self.memory = self.session.service("ALMemory")
        self.subscriberDialog = self.memory.subscriber("AkinatorDialog/answer")
        self.subscriberDialog.signal.connect(self.on_event_click)
        # TODO: Mettre une autre API
        self.url = "http://82.66.88.116:5080/akinator"

        # Init ALDialog service
        try:
            self.ALDialog = session.service("ALDialog")
            self.ALDialog.resetAll()
            self.ALDialog.setLanguage("French")
            # Loading the topics directly as text strings
            self.topic_name = self.ALDialog.loadTopic("/home/nao/.local/share/PackageManager/apps/Akinator/topfiles/Akinator.top")

            # Activating the loaded topics
            self.ALDialog.activateTopic(self.topic_name)

            # Starting the dialog engine - we need to type an arbitrary string as the identifier
            # We subscribe only ONCE, regardless of the number of topics we have activated
            self.ALDialog.subscribe('AkinatorDialog')

        except Exception as e:
            print ("Error was: ", e)

        # Init ALTabletService.
        self.tabletService = session.service("ALTabletService")
        self.tabletService.loadApplication("Akinator")
        self.tabletService.cleanWebview()
        self.tabletService.reloadPage(1)
        self.tabletService.showWebview()


        # Connect the event callback.
        self.subscriber = self.memory.subscriber("answer")
        self.subscriber.signal.connect(self.on_event_vocal)

    def on_event_vocal(self, value):
        """
        Callback for answers in Dialog
        """
        self.answer_question_with_api(value)

    def on_event_click(self, value):
        """
        Callback for answers on click
        """
        # self.answer_question_with_api(self, value)
        # self.animated_speech.say("Tu as dit " + value)

        # response = requests.get('http://82.66.88.116:5080/ping', headers={'accept': 'text/plain'})
        self.answer_question_with_api(value)


    def answer_question_with_api(self, value):

        print("User input : " + value)

        # if value == "start" and self.hasStarted == False:
        if value == "start" :
            url = self.url + '/start'
            # self.hasStarted = True
        elif value != "start" :
            url = self.url + '/response/' + value

        info = requests.get(url)
        info_json = info.json()
        self.isFinished = info_json["isGuess"]
        self.question = info_json["question"]

        # data = str(ques).replace("'", "").replace("u", "")
        self.memory.insertData("question", self.question)

        if (self.isFinished):
            self.animated_speech.say("Je sais ! Ton personnage est :")
            self.animated_speech.say(self.question)

            print("Avant requete")

            url_problemes = "http://82.66.88.116:5080/anecdote/" + self.question
            anecdote = requests.get(url_problemes)
            print("Apres requete")

            anecdote_text = anecdote.text

            self.animated_speech.say(anecdote_text)

        else :
            self.animated_speech.say(self.question)

        try :
            print(self.question)
        except :
            print("char chelou")

    def run(self):
        """
        Loop on, wait for events until manual interruption.
        """
        print ("Starting HumanGreeter")
        try:
            while True:
                fi = raw_input('H: ')
                #print fi
                self.ALDialog.forceInput(fi)
                self.ALDialog.forceOutput()
                time.sleep(1)
        except KeyboardInterrupt:
            print ("Interrupted by user, stopping HumanGreeter")

            # stopping the dialog engine
            self.ALDialog.unsubscribe('AkinatorDialog')
            # Deactivating the topic
            self.ALDialog.deactivateTopic(self.topic_name)
            # now that the dialog engine is stopped and there are no more activated topics,
            # we can unload our topic and free the associated memory
            self.ALDialog.unloadTopic(self.topic_name)
            #stop
            sys.exit(0)


def main():
    """
    Init Akinator Module and registers it
    """

    app = qi.Application(url="tcp://10.50.90.103:9559")
    app.start()

    s = app.session
    my_module = AkinatorModule(s)
    s.registerService("Akinator", my_module)

    app.run()

if __name__ == "__main__":
    main()