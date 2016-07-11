cp -f ../Ext/Icons_04_CB/Finals2/plockb.ico ../KeePass/KeePass.ico
cp -f ../Ext/Icons_04_CB/Finals2/plockb.ico ../KeePass/Resources/Images/KeePass.ico

sed 's!<SignAssembly>true</SignAssembly>!<SignAssembly>false</SignAssembly>!g' ../KeePass/KeePass.csproj > ../KeePass/KeePass.csproj.new
sed 's! ToolsVersion="3.5"!!g' ../KeePass/KeePass.csproj.new > ../KeePass/KeePass.csproj.new2
cat ../KeePass/KeePass.csproj.new2 | grep -v 'sgen\.exe' > ../KeePass/KeePass.csproj
rm -f ../KeePass/KeePass.csproj.new2
rm -f ../KeePass/KeePass.csproj.new

sed 's!<SignAssembly>true</SignAssembly>!<SignAssembly>false</SignAssembly>!g' ../KeePassLib/KeePassLib.csproj > ../KeePassLib/KeePassLib.csproj.new
sed 's! ToolsVersion="3.5"!!g' ../KeePassLib/KeePassLib.csproj.new > ../KeePassLib/KeePassLib.csproj
rm -f ../KeePassLib/KeePassLib.csproj.new

# cat ../KeePass.sln | grep -v 'DC15F71A-2117-4DEF-8C10-AA355B5E5979' | uniq > ../KeePass.sln.new
# mv -f ../KeePass.sln.new ../KeePass.sln
