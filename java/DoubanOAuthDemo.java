import java.io.*;
import java.text.*;
import java.util.*;
import javax.servlet.*;
import javax.servlet.http.*;
import com.google.gdata.client.douban.DoubanService;
import com.google.gdata.data.PlainTextConstruct;
import com.google.gdata.util.ServiceException;

/**
 * Douban API Java client OAuth demo.
 *
 * @author subdragon
 */

public class DoubanOAuthDemo extends HttpServlet {


    public void doGet(HttpServletRequest request,
                      HttpServletResponse response)
        throws IOException, ServletException
    {
        response.setContentType("text/html");
        PrintWriter out = response.getWriter();


	    String title = "OAuth Java is Simple";
        String requestToken = request.getParameter("oauth_token");

        out.println("<h1>" + title + "</h1>");
        String apiKey = "059ef56f6b705e1210dce04e42511a36";
        String secret = "006ba4a489916c13";

        out.println("<html>");
        out.println("<head>");
	    out.println("<title>" + title + "</title>");
        out.println("</head>");
        out.println("<body bgcolor=\"white\">");

        DoubanService myService = new DoubanService("subApplication", apiKey, secret);

        if(requestToken != null) {
            // step2 : user give the authorization and prepare to get the request token
            out.println(requestToken);
            try{
                Cookie[] cookies = request.getCookies();
                if (cookies == null) {
                    out.println("request token secret not found in cookie");
                    return;
                }
                Cookie c = cookies[0];
                // set request token and token secret
                myService.setRequestTokenSecret(c.getValue());
                myService.setRequestToken(requestToken);

                // get request token
                myService.getAccessToken();

                myService.createSaying(new PlainTextConstruct(title));
                out.println("<br/> You have just posted a message to douban saying");
            }catch (IOException e) {
                out.println("Oops! networking error!");
                e.printStackTrace();
            } catch (ServiceException e) {
                out.println("Oops! wrong request token!");
                e.printStackTrace();
            }

        } else {

            out.println(request.getRequestURL());
            out.println("<br/>");
            // step1 : generate authorization url and set the callback url to the current url
            out.println("<a href=" + myService.getAuthorizationUrl(request.getRequestURL().toString()) + "> click me to jump to the oauth page.</a>");
            // put request secret in cookie
            Cookie c = new Cookie("secret", myService.getRequestTokenSecret());
            response.addCookie(c);
        }

        out.println("</body>");
        out.println("</html>");
    }
}



