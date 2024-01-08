request = http.get("https://chestapp.azurewebsites.net/Music/convert/" .. arg[1], nil, true)
print(textutils.serialize(request.getResponseHeaders()))
print(request.getResponseCode())
local handle = request.readAll()
local file = fs.open("Song1.dfpwm","wb") --opens the file defined in 'saveTo' with the permissions to write.
file.write(handle) --writes all the stuff in handle to the file defined in 'saveTo'
file.close()
request.close()