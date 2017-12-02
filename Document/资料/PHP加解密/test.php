<?php

    // $f1=fopen("doublex.key","r");
    // $data1=fread($f1,8192);
    // fclose($f1);
    // $key1=openssl_pkey_get_private($data1);
    // var_dump($key1);

    // $f2=fopen("private.key","r");
    // $data2=fread($f2,8192);
    // fclose($f2);
    // $key2=openssl_pkey_get_public($data2);
    // var_dump($key2);

    $config = array(  
        "config" => "C:/xampp/php/extras/openssl/openssl.cnf"
    );  
          
    //创建公钥和私钥   返回资源  
    $res = openssl_pkey_new($config); 
    //从得到的资源中获取私钥    并把私钥赋给$<span style="font-family: Arial, Helvetica, sans-serif;">privKey</span>  
    openssl_pkey_export($res, $privKey);  
      
    //<span style="font-family: Arial, Helvetica, sans-serif;">从得到的资源中获取公钥    返回公钥 </span><span style="font-family: Arial, Helvetica, sans-serif;">$pubKey</span><span style="font-family: Arial, Helvetica, sans-serif;">  
    $pubKey = openssl_pkey_get_details($res);  
      
    //$pubKey = str_replace("\r","",str_replace("\n","",$pubKey["key"]));
    $pubKey = $pubKey["key"];
    var_dump($pubKey);  
    var_dump(openssl_pkey_get_public($pubKey));


    echo'</br>';
    echo'</br>';


    $dx_pub=file_get_contents("doublex.key");
    $dx_pre=file_get_contents("private.key");
    var_dump($dx_pub);  
    
    echo'</br>';
    echo'</br>';
    var_dump(openssl_pkey_get_public(file_get_contents("doublex.key")));
    var_dump(openssl_pkey_get_private(file_get_contents("private.key")));

    echo'</br>';
    echo'</br>';

    //{"Product":"DoubleX.Upload","Edition":"Basic","Email":"demo@demo.com","Mobile":"XXXXXXXXXXX","Mac":"XX-XX-XX-XX-XX-XX","Cpu":"XXXXXXXXXXXXXXXX","Times":"0","Date":"1900-01-01","IsTrial":true}
    $arr = array ('Product'=>'DoubleX.Upload',
    'Edition'=>'Basic',
    'Email'=>'demo@demo.com',
    'Mobile'=>'XXXXXXXXXXX',
    'Mac'=>'XX-XX-XX-XX-XX-XX',
    'Cpu'=>'XXXXXXXXXXXXXXXX',
    'Times'=>'0',
    'Date'=>'1900-01-01',
    'IsTrial'=>true);
    $json=json_encode($arr);

    echo '加密内容：'.$json;

    echo'</br>';
    echo'</br>加密结果：';

    //openssl_public_encrypt($data, $encrypted, $dx_pub);  //公钥加密  
    //openssl_private_encrypt($json,$encrypted,$dx_pre);   //私钥加密  

    $encrypted='';
    foreach(str_split($json,117) as $chunk){
        openssl_private_encrypt($chunk,$estr,$dx_pre);   //私钥加密  
        $encrypted.=$estr;
    }

    echo'</br>';
    echo base64_encode($encrypted); 

    echo'</br>';
    echo'</br>工具内容：';
    echo'</br>';
    echo 'gu1U2ZJNmSSgHqVfG1Er7Li6LMke0IxXzHNH5zBLnSW8M66L1Dw5UqOSMyys5/YujC4ykf8wv9Thyv6HR+aepsi1cyuulupuUG9iO3VhSqVl14gYJzgN2+tC+3H8JYjHFNHjw6JgWjo1tWBWywKVHlSIwpq1Er5p7Z2Vwqc2gd08kHSDp1UWpph79vjjCPcl8444JI1fzOCLB+Apq92KpDmAVkSXRbrsr1qtjxUU9IVM/H90ZVYuPa1yU1T6Nn0PgvA89Ye52/bIKsaceoKHmmam0z2T2+kgKMONdpfMA8DNPQQ9o6JYaKc0b5Ymkt7KMyzXLo1BKq4fG08Igmri9w==';

    echo'</br>';

?>

