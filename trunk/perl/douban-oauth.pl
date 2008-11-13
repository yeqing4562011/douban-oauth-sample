#!/usr/bin/perl 
#===============================================================================
#
#         FILE:  douban-oauth.pl
#
#        USAGE:  perl douban-oauth.pl
#
#  DESCRIPTION:  本脚本仅仅是一个使用豆瓣OAuth进行认证的示例脚本
#  		使用了CPAN(cpan.org) 上已有的OAuth::Lite::Consumer模块
#  		如需了解更多，请访问：
#  		http://search.cpan.org/~lyokato/OAuth-Lite-1.14/lib/OAuth/Lite/Consumer.pm
#
#      OPTIONS:  ---
# REQUIREMENTS:  在运行此脚本之前，请先从CPAN安装OAuth::Lite::Consumer模块，
# 		安装步骤如下：
# 		Unix+Perl:
# 		#cpan install OAuth::Lite::Consumer
# 		Windows+ActivePerl:
# 		c:\>ppm install OAuth::Lite::Consumer
#         BUGS:  ---
#        NOTES:  ---
#       AUTHOR:  lobatt (LICH), <lob4tt@gmail.com>
#      COMPANY:
#      VERSION:  1.0
#      CREATED:  2008-11-13 13:04:11 CST
#     REVISION:  ---
#===============================================================================

use strict;
use warnings;
use Data::Dumper;
use OAuth::Lite::Consumer;

#for application developer: replace blow with your own key and secret
#注意：请在开发程序时使用自己的key和secret
#具体参见：http://www.douban.com/service/apidoc/auth

my $consumer_key    = '072c3f9543f76b251584bebfcbe6416f';
my $consumer_secret = '696e5970864d0684';

my $consumer = OAuth::Lite::Consumer->new(
    consumer_key       => $consumer_key,
    consumer_secret    => $consumer_secret,
    site               => q{http://www.douban.com},
    request_token_path => q{/service/auth/request_token},
    access_token_path  => q{/service/auth/access_token},
    authorize_path     => q{/service/auth/authorize},
);


#请求request_token

my $request_token = $consumer->get_request_token();

#提示用户将url贴到浏览器中进行授权
print
  "Please Copy & Paste the URL below to your Web Browser,and click 'Agree'\n";
print
  "请将下面的url复制粘贴到您的浏览其中进行授权\n\n";
print $consumer->url_to_authorize . "?"
  . $consumer->oauth_response->{'_content'} . "\n\n";
print "Then press any key to Continue\n";
<STDIN>;
print "Please Wait...\n";
print "请稍候...\n";


#利用request_token来获取access_token
my $access_token = $consumer->get_access_token( token => $request_token );

#使用获得的access_token来访问豆瓣服务，这里是添加一条广播

my $res = $consumer->request(
    method  => 'POST',
    url     => qq{http://api.douban.com/miniblog/saying},
    token   => $access_token,
    headers => [ 'Content-Type' => q{application/atom+xml} ],
    content => qq{<entry xmlns:ns0="http://www.w3.org/2005/Atom" xmlns:db="http://www.douban.com/xmlns/"><content>Perl OAuth 认证成功</content></entry>},
);

#print Dumper($consumer);
print $res->content;

