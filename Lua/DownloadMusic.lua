local LibDeflate = require("LibDeflate")

local fileDownload, reason = http.get(arg[1])

if(fileDownload) then
    local handle = fileDownload.readAll()

    local compress_deflate = LibDeflate:CompressDeflate(handle, {level = 5})

    local file = fs.open("Song","w") --opens the file defined in 'saveTo' with the permissions to write.
    file.write(compress_deflate) --writes all the stuff in handle to the file defined in 'saveTo'
    file.close() --remember to close the file! 
else 
    print(reason) 
end

