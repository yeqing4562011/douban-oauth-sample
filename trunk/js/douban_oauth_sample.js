/*本示例程序提供最基本的Douban OAuth认证的JavaSciprt示例代码
 
其中oauth目录中oauth.js和sha1.js代码来源于 http://code.google.com/p/oauth/

本示例程序运行于Adobe AIR环境——允许跨域访问，如果你希望改造本程序将其运行在浏览器中，请确定你自己了解

关于浏览器跨域访问限制的相关问题

本示例程序使用Mootools框架，因为Mootools封装了修改XHR headers的操作——对于豆瓣OAuth认证访问受限资源是

必须的。当然JQuery、Dojo、Extjs等其他框架也可以完成同样的功能。
 
更多其他语言版本的Douban OAuth认证示例代码在 http://code.google.com/p/douban-oauth-sample/ 上提供

有任何疑问，可以到 http://www.douban.com/group/dbapi/ 上提问*/

/*
 * Douban OAuth认证包括以下四步内容
 * 
 * 1. 获取Request Token，该步骤使用API Key和API Key Secret签名
 * 2. 用户确认授权
 * 3. 换取Access Token，该步骤使用API Key、API Key Secret、Request Token和Request Token Secret签名
 * 4. 访问受限资源，该步骤使用API Key、API Key Secret、Access Token和Access Token Secret签名
 * 
 */

var api_key = "";
var api_key_secret = "";
var request_token = "";
var request_token_secret = "";
var access_token = "";
var access_token_secret = "";

var signature_method = "HMAC-SHA1";

var request_token_uri = "http://www.douban.com/service/auth/request_token";
var access_token_uri = "http://www.douban.com/service/auth/access_token";
var authorization_uri = "http://www.douban.com/service/auth/authorize?oauth_token=";
var miniblog_uri = "http://api.douban.com/miniblog/saying";

//1. 获取Request Token，该步骤使用API Key和API Key Secret签名
function getRequestToken(cb, fcb){
	var message = {
	    method: "GET",
	    action: request_token_uri,
	    parameters: {
	        oauth_consumer_key: api_key,
					oauth_signature_method: signature_method,
	        oauth_signature: "",
	        oauth_timestamp: "",
	        oauth_nonce: ""
	    }
  }
  
  // 签名
  OAuth.setTimestampAndNonce(message);
  OAuth.SignatureMethod.sign(message, {
      consumerSecret: api_key_secret
  })
        
	var req = new Request({
			//构造请求Request Token的url
      url: message.action + '?' + new Hash(OAuth.getParameterMap(message.parameters)).toQueryString(),
      method: message.method,
      onSuccess: (function(responseText){
      		//解析返回的Request Token和Request Token Secret
          var responseObj = OAuth.getParameterMap(OAuth.decodeForm(responseText));
          request_token = responseObj.oauth_token
			    request_token_secret = responseObj.oauth_token_secret
			    if ($type(cb) == 'function') cb(request_token, request_token_secret);
      }).bind(this),
      onFailure: (function(xhr){
      	if ($type(fcb) == 'function') fcb(xhr);
      }).bind(this)
  }).send()
}

// 2. 用户确认授权
function getUserAuthorizationURL(){  	
  	//生成引导用户授权的url
    return authorization_uri + request_token;
}

// 3. 换取Access Token，该步骤使用API Key、API Key Secret、Request Token和Request Token Secret签名
function getAccessToken(cb, fcb){
    var message = {
        method: "GET",
        action: access_token_uri,
        parameters: {
            oauth_consumer_key: api_key,
            oauth_token: request_token,
						oauth_signature_method: signature_method,
            oauth_signature: "",
            oauth_timestamp: "",
            oauth_nonce: ""
        }
    }
    
    // 签名
    OAuth.setTimestampAndNonce(message);
    OAuth.SignatureMethod.sign(message, {
        consumerSecret: api_key_secret,
        tokenSecret: request_token_secret,
    });

    var req = new Request({
	  		//构造请求Access Token的url
	      url: message.action + '?' + new Hash(OAuth.getParameterMap(message.parameters)).toQueryString(),
	      method: message.method,
	      onSuccess: (function(responseText){
	      		//解析返回的Request Token和Request Token Secret
	          var responseObj = OAuth.getParameterMap(OAuth.decodeForm(responseText));
	          access_token = responseObj.oauth_token
	          access_token_secret = responseObj.oauth_token_secret
						if ($type(cb) == 'function') cb(access_token, access_token_secret);
	      }).bind(this),
	      onFailure: (function(xhr){
	      		if ($type(fcb) == 'function') fcb(xhr);
	      }).bind(this)
    }).send()
}

// 4. 访问受限资源，该步骤使用API Key、API Key Secret、Access Token和Access Token Secret签名
function sendMiniblog(cb, fcb){
		var message = {
		    method: "POST",
		    action: miniblog_uri,
		    parameters: {
		        oauth_consumer_key: api_key,
		        oauth_token: access_token,
						oauth_signature_method: signature_method,
		        oauth_signature: "",
		        oauth_timestamp: "",
		        oauth_nonce: ""
			  }
	  }
	  // 签名
	  OAuth.setTimestampAndNonce(message);
	  OAuth.SignatureMethod.sign(message, {
	      consumerSecret: api_key_secret,
	      tokenSecret: access_token_secret,
	  });

		//构造OAuth头部
		var oauth_header = "OAuth realm=\"\", oauth_consumer_key=";
		oauth_header += message.parameters.oauth_consumer_key + ', oauth_nonce=';
		oauth_header += message.parameters.oauth_nonce + ', oauth_timestamp=';
		oauth_header += message.parameters.oauth_timestamp + ', oauth_signature_method=HMAC-SHA1, oauth_signature=';
		oauth_header += message.parameters.oauth_signature + ', oauth_token=';
		oauth_header += message.parameters.oauth_token;
	
		//构造请求
	  var request_body = "<entry xmlns:ns0=\"http://www.w3.org/2005/Atom\" xmlns:db=\"http://www.douban.com/xmlns/\">";
		request_body += "<content>Javascript OAuth认证成功</content>";
		request_body += "</entry>";
		
		var req = new Request({
	      url: message.action,
	      method: message.method,
	      //设置Http Request Headers
				headers: {'Authorization': oauth_header, 'content-type': 'application/atom+xml'},
	      data: request_body,
	      onSuccess: (function(responseText){
						if ($type(cb) == 'function') cb(responseText);
	      }).bind(this),
	      onFailure: (function(xhr){
	      		if ($type(fcb) == 'function') fcb(xhr);
	      }).bind(this)
	  }).send()
}
