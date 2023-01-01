import requests
import random
import time
from multiprocessing import Process

def str_time_prop(start, end, time_format, prop):
    stime = time.mktime(time.strptime(start, time_format))
    etime = time.mktime(time.strptime(end, time_format))
    ptime = stime + prop * (etime - stime)
    return time.strftime(time_format, time.localtime(ptime))


def random_date(start="2022-12-27T00:00:01", end="2022-12-31T23:59:59", prop=random.random()):
    return str_time_prop(start, end, '%Y-%m-%dT%H:%M:%S', prop)


def send():
    url = 'http://localhost:30000/update'
    while True:
        data = {'timestamp': random_date(), 'temperature': str(round(random.random()*100, 2))}
        headers = {'sensor-id': str(random.randint(0, 100))}
        requests.post(url, json=data, headers=headers)
        time.sleep(100)

def main():
    Pros = []
    for i in range(0,100):
        p = Process(target=send)
        Pros.append(p)
        p.start()
        p.join()


if __name__ == '__main__':
    main()
