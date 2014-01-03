__author__ = 'emilio.torrens'

from client import Client

data = Client.instance().get('HOLA_1')
data2 = data


data = Client.instance().get('HOLA_2')
data2 = data

keys = ['HOLA_1', 'HOLA_2']
data = Client.instance().get_list(keys)
data2 = data






