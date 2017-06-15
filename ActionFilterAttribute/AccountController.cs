#if PROD    
[RequireHttps]
#endif
 [Authorize] 
 [InitializeSimpleMembership] 
 [LogAction]
 public class AccountController : Controller    {
      public AccountController()            : this(new FormsAuthenticationService(), new UserAuthenticator())        
      {        
      //this.FormsAuth = new FormsAuthenticationService();       
      }
      public interface IFormsAuthentication    {       
         void SignIn(string userName, bool createPersistentCookie);      
         void SignOut();   
      }
      public class FormsAuthenticationService : IFormsAuthentication    {
      public void SignIn(string userName, bool createPersistentCookie)     
      {
      FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);      
      }
      public void SignOut()        {
      FormsAuthentication.SignOut();       
      }
      
 }
