"# api-share-files" 
Main workflow / functionality:
  1. User can register (using email + password) and then log in using those email and password
  2. Authorized users have ability to:
        a. Upload a file. Users should be able to specify if they want the file to be automatically deleted
        once it’s downloaded. After uploading, the user will receive the file URL (file can only be
        downloaded from this URL). It is important to have some sort of file urls protection (at the very
        least, the urls should not have a simple numeric identifier like /api/files/1; a better level of
        protection is welcomed)
        b. Upload a text (string). Users should be able to specify if they want the information to be
        automatically deleted once it’s accessed. After sending the text, the user should receive the
        text URL (the text can only be accessed from this URL). It is important to have some sort of urls
        protection (at the very least, the urls should not have a simple numeric identifier like /api/files/1;
        a better level of protection is welcomed)
  
  3. Anonymous users can:
        a. Access files/texts using the URLs generated from steps 2.a, 2.b
