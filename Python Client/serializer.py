__author__ = 'emilio.torrens'

import simplejson as json
import string_compressor


class Serializer:
    def __init__(self):
        self._compression_enabled = True

    def string_2_object(self, data):

        if self._compression_enabled:
            data = string_compressor.decompress(data)

        dict_data = json.loads(data)
        if isinstance(dict_data, list):
            result = []
            for item in dict_data:
                result.append(Serializer.dic_2_obj(item))
            return result
        else:
            return Serializer.dic_2_obj(dict_data)

    @staticmethod
    def dic_2_obj(d):
        top = type('serialized_object', (object,), d)
        seqs = tuple, list, set, frozenset
        for i, j in d.items():
            if isinstance(j, dict):
                setattr(top, i, Serializer.dic_2_obj(j))
            elif isinstance(j, seqs):
                setattr(top, i,
                        type(j)(Serializer.dic_2_obj(sj) if isinstance(sj, dict) else sj for sj in j))
            else:
                setattr(top, i, j)
        return top