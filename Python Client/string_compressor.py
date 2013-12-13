__author__ = 'emilio.torrens'

import base64
from gzip import GzipFile
from cStringIO import StringIO


def decompress(data):
    b = map(ord, base64.b64decode(data))
    b2 = bytearray(b[4:])
    result = GzipFile(fileobj=StringIO(b2)).read()
    return result
