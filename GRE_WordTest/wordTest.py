#settings
#File that contains the wordlist
FILE_NAME = "/home/adwait/gre.xls"

#sheet number in the excel (indexing statrs from 0)
SHEET_NUMBER = 0

#respective column numbers in the sheet (indexing statrs from 0)
WORD_COLUMN = 0 
MEANING_COLUMN = 1
OTHER_INFO_COLUMN = 2

#size of history to be maintained
HISTORY_SIZE = 60

#debug flag
DEBUG = False
#DEBUG = True

#-------------------------------------------------------------
import random
import xlrd

book = xlrd.open_workbook(FILE_NAME)
sheet = book.sheet_by_index(SHEET_NUMBER)
nlines = 0
nlines = sheet.nrows
#indexing starts from 0 so we need to go till line n-1 
nlines = nlines - 1
if DEBUG:
    print "\nTotal lines used : " + str(nlines)

#function checks if value is in the historyList
#if true then returns false
#else includes value to historyList and retuens True
def checkHistory(historyList, value):
    if DEBUG:
        print "Value : " + str(value)
        print "History : " + str(historyList)
    for item in historyList:
        if item == value:
            return False

    if len(historyList) == HISTORY_SIZE:
        #delete first item (LRU)
        historyList.pop(0)
    #add new word to history
    historyList.append(value)

    return True

#start main loop
if nlines > 0:
    exit = "n"
    cline = 0
    history = list()
    while exit != "Y":
        newWord = False
        while newWord == False:
            #select random word
            cline = random.randint(1, nlines)
            #check if it's in the history
            #if true then add to history else get another word
            newWord = checkHistory(history,cline)
            print "\nWord : " + sheet.cell_value(rowx = cline, colx = WORD_COLUMN)
            #take user answer (no comparison done. Just checking my spellings)
            answer = raw_input("Your answer : ")
            #show answer from file
            print "File answer: " + sheet.cell_value(rowx = cline, colx = MEANING_COLUMN)
            print "Hints :" + sheet.cell_value(rowx = cline, colx = OTHER_INFO_COLUMN)
            #go to next word or quit
            exit = raw_input("Quit (Y):")

print "Thanks for using my stuff. Copyright Adwait Kulkarni."
