from http.server import BaseHTTPRequestHandler, HTTPServer
import json
import urllib

def GetDataIndex(target_data_id):
    index = 0
    for player_data in player_data_dict['Data']:
        if player_data['DataId'] == target_data_id:
            return index
        else:
            index += 1
    return -1

def DeleteData(target_data_index):
    del player_data_dict['Data'][target_data_index]
    return;

player_data_json_file_path = 'PlayerDataBase.json'
player_data_string = None
with open(player_data_json_file_path) as f:
    player_data_string = f.read()

player_data_dict = json.loads(player_data_string)
port = 9999
buffer = None

class SimpleHTTPRequestHandler(BaseHTTPRequestHandler):
    def do_GET(self):
        print(urllib.parse.urlparse(self.path))
        # 헤더 작성이 반드시 먼저 되어야 함
        self.send_response(200)
        self.send_header('Content-Type', 'application/json')
        self.end_headers()
        
        parse_result = urllib.parse.urlparse(self.path)
        querys = urllib.parse.parse_qs(parse_result.query)
        
        if '/request/playerData' in parse_result.path:
            # python dictionary(json))에서는 "와 ' 둘 다 해석가능하지만, C# json에서는 "만 해석이 가능하다
            self.wfile.write(str(player_data_dict).replace("'", '"').replace("True", "true").replace("False", "false").encode('utf-8'))
        elif '/delete/playerData' in parse_result.path:
            # querys의 value는 리스트형. 
            if 'playerDataId' in querys and len(querys['playerDataId']) > 0:
                player_data_id = int(querys['playerDataId'][0])
                to_delete_index = GetDataIndex(player_data_id)
                DeleteData(to_delete_index)
        return
    
    def do_POST(self):
        print("Post: " + self.path)
        content_len = int(self.headers.get('content-length', 0))
        post_body = self.rfile.read(content_len)
        decoded_json_string = urllib.parse.unquote(post_body.decode())
        data = json.loads(decoded_json_string)
        status_code = 500
#         print("data " + str(data))
        if "Data" in data:
            if data["Data"]:
                existing = False;
                for i in range(len(player_data_dict["Data"])):
                    player_data = player_data_dict["Data"][i]
                    if player_data["DataId"] == data["Data"][0]["DataId"] and player_data['Status']['Name'] == data["Data"][0]['Status']['Name']:
                        # 동일한 플레이어면 덮어쓰기
                        existing = True
                        player_data_dict["Data"][i] = data["Data"][0]
                        print("Updated Player: " + data["Data"][0]['Status']['Name'])
                        break
                if not existing:
                    # 새로 추가
                    player_data_dict["Data"].append(data["Data"][0])
                    print("New saved Player: " + data["Data"][0]['Status']['Name'])
                status_code = 200 
        self.send_response(status_code)
        self.end_headers()
        return

httpd = HTTPServer(('localhost', port), SimpleHTTPRequestHandler)
print(f'Server running on port:{port}')
try:
    print("server start...")
    httpd.serve_forever()
except KeyboardInterrupt:
    httpd.server_close()
    print("server close...")