import unittest
import moto

from src import get_maps


@mock_s3
def test_my_model_save():
    conn = boto.connect_s3()
    conn.create_bucket('mybucket')

    model_instance = MyModel('steve', 'is awesome')
    model_instance.save()

    assert conn.get_bucket('mybucket').get_key('steve').get_contents_as_string() == 'is awesome'