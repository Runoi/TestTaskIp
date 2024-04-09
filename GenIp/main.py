from faker import Faker
from faker.providers import internet

def gen_ip(long):
    fake = Faker()
    fake.add_provider(internet)
    ip_list = []
    i = 0
    while i < long: 
                   
        ip = fake.ipv4_private()
        date = fake.date_between(start_date = '-5y', end_date = 'today')
        time = fake.time()
        full_str = ip + " " + str(date) +" "+ str(time)+ "\n"
        ip_list.append(full_str)
        i+=1
       
    return ip_list
    

with open("ip.txt","w") as file:
    ip = gen_ip(500000)
    file.writelines(ip)
        

