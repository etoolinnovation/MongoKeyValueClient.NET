__author__ = 'emilio.torrens'

from pymongo import MongoClient
import base64
from gzip import GzipFile
from cStringIO import StringIO


connection = MongoClient('mongodb://127.0.0.1')
db = connection.MongoKeyValueTest

result = db.CacheData.find({})

for r in result:
    print r['_id']
    y = r['Data']
    print y

    b = map(ord, base64.b64decode(y))
    b2 = bytearray(b[4:])
    body = GzipFile(fileobj=StringIO(b2)).read()

    print body







