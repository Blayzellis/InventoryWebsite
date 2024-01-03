local LibDeflate = require("LibDeflate")

local fileDownload, reason = http.get("http://162.231.184.144:8080/Music/convert/" .. arg[1], nil, true)

if(fileDownload) then
    local handle = fileDownload.readAll()

    local compress_deflate = LibDeflate:CompressDeflate(handle, {level = 1})

    local file = fs.open("Song1.dfpwm","wb") --opens the file defined in 'saveTo' with the permissions to write.
    file.write(compress_deflate) --writes all the stuff in handle to the file defined in 'saveTo'
    file.close() --remember to close the file! 
else 
    print(reason) 
end

