using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1Beta1;
using Grpc.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace WebApi.Firestore.Controllers
{
    public class HomeController : Controller
    {

        public HomeController()
        {
            //FirestoreDb
        }

        /// <summary>
        /// Creates a custom token to be used with apps to sign in using firebase custom authentication
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> CreateToken()
        {
            var uid = Guid.NewGuid().ToString();

            string project = "ainfirestoreproject";
            var mappedPath = Server.MapPath("/Certificate/ainfirestoreproject-139fe45328fd.json");
            var credential = GoogleCredential.FromFile(mappedPath);
           
            AppOptions a = new AppOptions()
            {
                Credential = credential,
                ProjectId = project,
            };

            if (FirebaseAuth.DefaultInstance == null)
            {
                FirebaseApp.Create(a);
            }

            string customToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(uid);
            ViewData["Token"] = customToken;
            return View();
        }

        /// <summary>
        /// Fetches document data from firestore db
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            string project = "ainfirestoreproject";
            var mappedPath = Server.MapPath("/Certificate/ainfirestoreproject-139fe45328fd.json");
            var credential = GoogleCredential.FromFile(mappedPath);
            var channel = new Grpc.Core.Channel(
                    FirestoreClient.DefaultEndpoint.ToString(),
                    credential.ToChannelCredentials());
            var client = FirestoreClient.Create(channel);
            var db = FirestoreDb.Create(project, client: client);
            System.Diagnostics.Debug.WriteLine("Created Cloud Firestore client with project ID: {0}", project);

            CollectionReference usersRef = db.Collection("tests");
            var snapshot = await usersRef.GetSnapshotAsync();


            return View(snapshot);
        }

        /// <summary>
        /// Creates a new data in firestore db
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> CreateUser()
        {
            string project = "ainfirestoreproject";
            var db = FirestoreDb.Create(project);
            System.Diagnostics.Debug.WriteLine("Created Cloud Firestore client with project ID: {0}", project);

            var g = Guid.NewGuid().ToString();
            DocumentReference docRef = db.Collection("tests").Document(g);
            Dictionary<string, object> user = new Dictionary<string, object>
            {
                { "First", g },
                { "Middle", "Mathison" },
                { "Last", "Turing" },
                { "Born", 1912 }
            };
            await docRef.SetAsync(user);
            return View();
        }
    }
}
