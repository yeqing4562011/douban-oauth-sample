<?php
require_once 'Zend/Gdata/DouBan.php';
require_once 'Zend/Gdata/DouBan/BroadcastingEntry.php';
require_once 'Zend/Gdata/App/Extension/Content.php';

/* your apikey and secret */
$APIKEY= '09d731d5647253a71d8595802d56faf5';
$API_SECRET= 'aa39148c1316435d';

/* the Douban client does everything */
$client = new Zend_Gdata_DouBan($APIKEY, $API_SECRET);

/* step 2: when it comes back from Douban auth page */
if(isset($_GET['oauth_token']))
{
    /* exchange the request token for access token */
    $key = $_COOKIE['key'];
    $secret = $_COOKIE['secret'];
    $result = $client->getAccessToken($key, $secret);
    $key = $result["oauth_token"];
    $secret = $result["oauth_token_secret"];
    if($key){

        /* access success, let's say something. */
        $client->programmaticLogin($key, $secret);
        echo 'logged in.';
        $entry = new Zend_Gdata_Douban_BroadcastingEntry();
        $content = new Zend_Gdata_App_Extension_Content('Oauth from PHP is easy.');
        $entry->setContent($content);
        $entry = $client->addBroadcasting("saying", $entry);
        echo '<br/>you just posted: '.$entry->getContent()->getText();
    }else{
        echo 'Oops, get access token failed';
    }
}

/* step 1: */
else
{
    /* first, get request token. */
    $result = $client->getRequestToken();
    $key = $result["oauth_token"];
    $secret = $result["oauth_token_secret"];

    /* save them somewhere, you'll need them in step 2. */
    setcookie('key',$result["oauth_token"],time()+3600);
    setcookie('secret',$result["oauth_token_secret"],time()+3600);

    /* get the auth url */
    $authurl = $client->getAuthorizationURL($key, $secret, 'http://'.$_ENV['HTTP_HOST'].$_ENV['REQUEST_URI']);
    echo '<a href="'.$authurl.'">click me to oauth it.</a>';
}

?>
