function mysplit (inputstr, sep)
  if sep == nil then
          sep = "%s"
  end
  local t={}
  for str in string.gmatch(inputstr, "([^"..sep.."]+)") do
          table.insert(t, str)
  end
  return t
end


local dfpwm = require("cc.audio.dfpwm")
local speakers = { peripheral.find("speaker") }
local volume = 50
local CHUNK_SIZE = 8 * 1024
local decoder = dfpwm.make_decoder()
print("Please paste a youtube link:")
local input = io.read()
local link = mysplit(input, "v=")[2]
print(link)
local fileDownload, reason = http.get("https://chestapp.azurewebsites.net/Music/convert/" .. link, nil, true)
print(reason)
local code = fileDownload.getResponseCode()
while(code ~= 200)do
  print(code)
  os.sleep(3)
  print("Downloading. . .")
  fileDownload, reason = http.get("https://chestapp.azurewebsites.net/Music/convert/" .. link, nil, true)
  code = fileDownload.getResponseCode()
end

local data = fileDownload.readAll()
while(true)
do
  for i = 1, #data, CHUNK_SIZE do
    buffer = decoder(data:sub(i, i + CHUNK_SIZE - 1))
    while not speakers[1].playAudio(buffer, volume) do
      os.pullEvent("speaker_audio_empty")
    end
  end

    

end
