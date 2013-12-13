__author__ = 'emilio.torrens'

from pymongo import MongoClient
from singleton import Singleton
import serializer

@Singleton
class Client:
    def __init__(self):
        connection = MongoClient('mongodb://127.0.0.1')
        db = connection.MongoKeyValueTest
        self._col = db['CacheData']
        self._serializer = serializer.Serializer()

    def get(self, key):
        data = self._col.find_one({'_id': key})
        return self._serializer.string_2_object(data['Data'])



