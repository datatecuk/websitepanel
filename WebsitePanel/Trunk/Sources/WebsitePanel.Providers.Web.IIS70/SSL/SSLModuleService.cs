﻿// - Redistributions in binary form  must  reproduce the  above  copyright  notice,
//   this list of conditions  and  the  following  disclaimer in  the documentation
//   and/or other materials provided with the distribution.
//
// - Neither  the  name  of  the  SMB SAAS Systems Inc.  nor   the   names  of  its
//   contributors may be used to endorse or  promote  products  derived  from  this
//   software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING,  BUT  NOT  LIMITED TO, THE IMPLIED
// WARRANTIES  OF  MERCHANTABILITY   AND  FITNESS  FOR  A  PARTICULAR  PURPOSE  ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL,  SPECIAL,  EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO,  PROCUREMENT  OF  SUBSTITUTE  GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)  HOWEVER  CAUSED AND ON
// ANY  THEORY  OF  LIABILITY,  WHETHER  IN  CONTRACT,  STRICT  LIABILITY,  OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE)  ARISING  IN  ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using Microsoft.Web.Administration;
using WebsitePanel.Providers.Common;
using WebsitePanel.Server.Utils;
using Microsoft.Web.Administration;
using System;
using System.Linq;
using CERTENROLLLib;
using CERTCLIENTLib;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using WebsitePanel.Providers.Web.Iis.Common;
using System.Security;
using WebsitePanel.Providers.Web.Iis.WebObjects;

namespace WebsitePanel.Providers.Web.Iis
{
	internal sealed class SSLModuleService : ConfigurationModuleService
	{
		public void GenerateCsr(SSLCertificate cert)
		{
			//  Create all the objects that will be required
			CX509CertificateRequestPkcs10 pkcs10 = new CX509CertificateRequestPkcs10Class();
			CX509PrivateKey privateKey = new CX509PrivateKeyClass();
			CCspInformation csp = new CCspInformationClass();
			CCspInformations csPs = new CCspInformationsClass();
			CX500DistinguishedName dn = new CX500DistinguishedNameClass();
			CX509Enrollment enroll = new CX509EnrollmentClass();
			CObjectIds objectIds = new CObjectIdsClass();
			CObjectId clientObjectId = new CObjectIdClass();
			CObjectId serverObjectId = new CObjectIdClass();
			CX509ExtensionKeyUsage extensionKeyUsage = new CX509ExtensionKeyUsageClass();
			CX509ExtensionEnhancedKeyUsage x509ExtensionEnhancedKeyUsage = new CX509ExtensionEnhancedKeyUsageClass();

			try
			{
				//  Initialize the csp object using the desired Cryptograhic Service Provider (CSP)
				csp.InitializeFromName("Microsoft RSA SChannel Cryptographic Provider");
				//  Add this CSP object to the CSP collection object
				csPs.Add(csp);

				//  Provide key container name, key length and key spec to the private key object
				//objPrivateKey.ContainerName = "AlejaCMa";
				privateKey.Length = cert.CSRLength;
				privateKey.KeySpec = X509KeySpec.XCN_AT_SIGNATURE;
				privateKey.KeyUsage = X509PrivateKeyUsageFlags.XCN_NCRYPT_ALLOW_ALL_USAGES;
				privateKey.ExportPolicy = X509PrivateKeyExportFlags.XCN_NCRYPT_ALLOW_PLAINTEXT_EXPORT_FLAG | X509PrivateKeyExportFlags.XCN_NCRYPT_ALLOW_EXPORT_FLAG;
				privateKey.MachineContext = true;

				//  Provide the CSP collection object (in this case containing only 1 CSP object)
				//  to the private key object
				privateKey.CspInformations = csPs;

				//  Create the actual key pair
				privateKey.Create();

				//  Initialize the PKCS#10 certificate request object based on the private key.
				//  Using the context, indicate that this is a user certificate request and don't
				//  provide a template name
				pkcs10.InitializeFromPrivateKey(X509CertificateEnrollmentContext.ContextMachine, privateKey, "");

				cert.PrivateKey = privateKey.ToString();
				// Key Usage Extension 
				extensionKeyUsage.InitializeEncode(
					CERTENROLLLib.X509KeyUsageFlags.XCN_CERT_DIGITAL_SIGNATURE_KEY_USAGE |
					CERTENROLLLib.X509KeyUsageFlags.XCN_CERT_NON_REPUDIATION_KEY_USAGE |
					CERTENROLLLib.X509KeyUsageFlags.XCN_CERT_KEY_ENCIPHERMENT_KEY_USAGE |
					CERTENROLLLib.X509KeyUsageFlags.XCN_CERT_DATA_ENCIPHERMENT_KEY_USAGE
				);

				pkcs10.X509Extensions.Add((CX509Extension)extensionKeyUsage);

				// Enhanced Key Usage Extension
				clientObjectId.InitializeFromValue("1.3.6.1.5.5.7.3.2");
				objectIds.Add(clientObjectId);
				serverObjectId.InitializeFromValue("1.3.6.1.5.5.7.3.1");
				objectIds.Add(serverObjectId);
				x509ExtensionEnhancedKeyUsage.InitializeEncode(objectIds);
				pkcs10.X509Extensions.Add((CX509Extension)x509ExtensionEnhancedKeyUsage);

				//  Encode the name in using the Distinguished Name object
				string request = String.Format(@"CN={0}, O={1}, OU={2}, L={3}, S={4}, C={5}", cert.Hostname, cert.Organisation, cert.OrganisationUnit, cert.City, cert.State, cert.Country);
				dn.Encode(request, X500NameFlags.XCN_CERT_NAME_STR_NONE);

				//  Assing the subject name by using the Distinguished Name object initialized above
				pkcs10.Subject = dn;

				// Create enrollment request
				enroll.InitializeFromRequest(pkcs10);

				enroll.CertificateFriendlyName = cert.FriendlyName;

				cert.CSR = enroll.CreateRequest(EncodingType.XCN_CRYPT_STRING_BASE64REQUESTHEADER);

			}
			catch (Exception ex)
			{
				Log.WriteError("Error creating CSR", ex);
			}
		}

		public SSLCertificate InstallCertificate(SSLCertificate cert, WebSite website)
		{
			CX509Enrollment response = new CX509EnrollmentClass();
			try
			{

				response.Initialize(X509CertificateEnrollmentContext.ContextMachine);
				response.InstallResponse(
					InstallResponseRestrictionFlags.AllowUntrustedRoot,
					cert.Certificate, EncodingType.XCN_CRYPT_STRING_BASE64HEADER,
					null
				);

				SSLCertificate servercert = (from c in GetServerCertificates()
											 where c.FriendlyName == cert.FriendlyName
											 select c).Single();

				cert.SerialNumber = servercert.SerialNumber;
				cert.ValidFrom = servercert.ValidFrom;
				cert.ExpiryDate = servercert.ExpiryDate;
				cert.Hash = servercert.Hash;
				cert.DistinguishedName = servercert.DistinguishedName;

				if (cert.IsRenewal && CheckCertificate(website))
				{
					DeleteCertificate(GetCurrentSiteCertificate(website), website);
				}

				AddBinding(cert, website);

			}
			catch (Exception ex)
			{


				Log.WriteError("Error adding SSL certificate", ex);
				cert.Success = false;
			}
			return cert;
		}

		public List<SSLCertificate> GetServerCertificates()
		{
			X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			//
			var certificates = new List<SSLCertificate>();
			//
			try
			{
				store.Open(OpenFlags.ReadOnly);
				//
				certificates = (from X509Certificate2 cert in store.Certificates
								let hostname = cert.GetNameInfo(X509NameType.SimpleName, false)
								select new SSLCertificate()
								{
									FriendlyName = cert.FriendlyName,
									Hostname = hostname,
									Hash = cert.GetCertHash(),
									SerialNumber = cert.SerialNumber,
									ValidFrom = DateTime.Parse(cert.GetEffectiveDateString()),
									ExpiryDate = DateTime.Parse(cert.GetExpirationDateString()),
									DistinguishedName = cert.Subject

								}).ToList();
			}
			catch (Exception ex)
			{
				Log.WriteError(
					String.Format("SSLModuleService is unable to get certificates from X509Store('{0}', '{1}') and complete GetServerCertificates call", store.Name, store.Location), ex);
				// Re-throw exception
				throw;
			}
			finally
			{
				store.Close();
			}
			//
			return certificates;
		}

		public SSLCertificate InstallPfx(byte[] certificate, string password, WebSite website)
		{
			X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			//
			SSLCertificate newcert = null, oldcert = null;
			// Ensure we perform operations safely and preserve the original state during all manipulations
			if (CheckCertificate(website))
				oldcert = GetCurrentSiteCertificate(website);
			//
			X509Certificate2 x509Cert = new X509Certificate2(certificate, password);

			#region Step 1: Register X.509 certificate in the store
			// Trying to keep X.509 store open as less as possible
			try
			{
				store.Open(OpenFlags.ReadWrite);
				//
				store.Add(x509Cert);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("SSLModuleService could not import PFX into X509Store('{0}', '{1}')", store.Name, store.Location), ex);
				// Re-throw error
				throw;
			}
			finally
			{
				store.Close();
			} 
			#endregion

			#region Step 2: Instantiate a copy of new X.509 certificate
			try
			{
				//
				store.Open(OpenFlags.ReadWrite);
				//
				newcert = new SSLCertificate
				{
					Hostname = x509Cert.GetNameInfo(X509NameType.SimpleName, false),
					FriendlyName = x509Cert.FriendlyName,
					CSRLength = Convert.ToInt32(x509Cert.PublicKey.Key.KeySize.ToString()),
					Installed = true,
					DistinguishedName = x509Cert.Subject,
					Hash = x509Cert.GetCertHash(),
					SerialNumber = x509Cert.SerialNumber,
					ExpiryDate = DateTime.Parse(x509Cert.GetExpirationDateString()),
					ValidFrom = DateTime.Parse(x509Cert.GetEffectiveDateString()),
				};
			}
			catch (Exception ex)
			{
				// Rollback X.509 store changes
				store.Remove(x509Cert);
				// Log error
				Log.WriteError("SSLModuleService could not instantiate a copy of new X.509 certificate. All previous changes have been rolled back.", ex);
				// Re-throw
				throw;
			}
			finally
			{
				store.Close();
			} 
			#endregion

			#region Step 3: Remove old certificate from the web site if any
			try
			{
				store.Open(OpenFlags.ReadWrite);
				// Check if certificate already exists, remove it.
				if (oldcert != null)
					DeleteCertificate(oldcert, website);
			}
			catch (Exception ex)
			{
				// Rollback X.509 store changes
				store.Remove(x509Cert);
				// Log the error
				Log.WriteError(
					String.Format("SSLModuleService could not remove existing certificate from '{0}' web site. All changes have been rolled back.", website.Name), ex);
				// Re-throw
				throw;
			}
			finally
			{
				store.Close();
			} 
			#endregion

			#region Step 4: Register new certificate with HTTPS binding on the web site
			try
			{
				store.Open(OpenFlags.ReadWrite);
				//
				AddBinding(newcert, website);
			}
			catch (Exception ex)
			{
				// Install old certificate back if any
				if (oldcert != null)
					InstallCertificate(oldcert, website);
				// Rollback X.509 store changes
				store.Remove(x509Cert);
				// Log the error
				Log.WriteError(
					String.Format("SSLModuleService could not add new X.509 certificate to '{0}' web site. All changes have been rolled back.", website.Name), ex);
				// Re-throw
				throw;
			}
			finally
			{
				store.Close();
			} 
			#endregion
			//
			return newcert;
		}

		public byte[] ExportPfx(string serialNumber, string password)
		{
			X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			store.Open(OpenFlags.ReadOnly);
			X509Certificate2 cert = store.Certificates.Find(X509FindType.FindBySerialNumber, serialNumber, false)[0];
			byte[] exported = cert.Export(X509ContentType.Pfx, password);

			return exported;
		}

		public void AddBinding(SSLCertificate certificate, WebSite website)
		{
			using (ServerManager sm = GetServerManager())
			{
				// Not sure why do we need to work with X.509 store here, so commented it out and lets see what happens
				X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
				store.Open(OpenFlags.ReadOnly);
				//
				List<ServerBinding> bindings = new List<ServerBinding>();
				// Retrieve existing site bindings to figure out what do we have here
				WebObjectsModuleService webObjSvc = new WebObjectsModuleService();
				bindings.AddRange(webObjSvc.GetSiteBindings(website.SiteId));
				// Look for dedicated ip
				bool dedicatedIp = bindings.Exists(binding => String.IsNullOrEmpty(binding.Host) && binding.IP != "*");
				//
				string bindingInformation;
				//
				bindingInformation = dedicatedIp ? string.Format("{0}:443:", website.SiteIPAddress)
												 : string.Format("{0}:443:{1}", website.SiteIPAddress, certificate.Hostname);
				//
				sm.Sites[website.SiteId].Bindings.Add(bindingInformation, certificate.Hash, store.Name);
				//
				store.Close();
				//
				sm.CommitChanges();
			}
		}

		public void RemoveBinding(SSLCertificate certificate, WebSite website)
		{
			using (ServerManager sm = GetServerManager())
			{
				Site site = sm.Sites[website.SiteId];

				Binding sslbind = (from b in site.Bindings
								   where b.Protocol == "https"
								   select b).Single();

				site.Bindings.Remove(sslbind);

				sm.CommitChanges();
			}
		}

		public SSLCertificate FindByFriendlyname(string name)
		{
			throw new NotImplementedException("Method not implemented");
		}

		public ResultObject DeleteCertificate(SSLCertificate certificate, WebSite website)
		{
			ResultObject result = new ResultObject();

			try
			{
				X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);

				store.Open(OpenFlags.MaxAllowed);

				X509Certificate2 cert =
					store.Certificates.Find(X509FindType.FindBySerialNumber, certificate.SerialNumber, false)[0];
				store.Remove(cert);

				store.Close();
				RemoveBinding(certificate, website);

				result.IsSuccess = true;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.AddError("", ex);
			}
			return result;
		}

		public SSLCertificate ImportCertificate(WebSite website)
		{
			SSLCertificate certificate = new SSLCertificate { Success = false };
			try
			{
				using (ServerManager sm = GetServerManager())
				{
					Site site = sm.Sites[website.SiteId];

					Binding sslbind = (from b in site.Bindings
									   where b.Protocol == "https"
									   select b).Single();


					certificate.Hash = sslbind.CertificateHash;

					X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);

					store.Open(OpenFlags.MaxAllowed);


					X509Certificate2 x509Cert = (from X509Certificate2 c in store.Certificates
												 where Convert.ToBase64String(c.GetCertHash()) == Convert.ToBase64String(certificate.Hash)
												 select c).Single();

					store.Close();

					certificate.Hostname = x509Cert.GetNameInfo(X509NameType.SimpleName, false);
					certificate.FriendlyName = x509Cert.FriendlyName;
					certificate.CSRLength = Convert.ToInt32(x509Cert.PublicKey.Key.KeySize.ToString());
					certificate.Installed = true;
					certificate.DistinguishedName = x509Cert.Subject;
					certificate.Hash = x509Cert.GetCertHash();
					certificate.SerialNumber = x509Cert.SerialNumber;
					certificate.ExpiryDate = DateTime.Parse(x509Cert.GetExpirationDateString());
					certificate.ValidFrom = DateTime.Parse(x509Cert.GetEffectiveDateString());
					certificate.Success = true;
				}
			}
			catch (Exception ex)
			{
				certificate.Success = false;
				certificate.Certificate = ex.ToString();
			}
			return certificate;
		}

		//Checks to see if the site has a certificate
		public bool CheckCertificate(WebSite website)
		{
			using (var sm = GetServerManager())
			{
				//
				Site site = sm.Sites[website.SiteId];
				// Just exit from the loop if https binding found
				foreach (Binding bind in site.Bindings.Where(bind => bind.Protocol == "https"))
					return true;
				//
				return false;
			}
		}

		public SSLCertificate GetCurrentSiteCertificate(WebSite website)
		{
			using (ServerManager sm = GetServerManager())
			{
				Site site = sm.Sites[website.SiteId];
				Binding sslbind = (from b in site.Bindings
								   where b.Protocol == "https"
								   select b).Single();

				byte[] currentHash = sslbind.CertificateHash;
				X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
				store.Open(OpenFlags.ReadOnly);

				X509Certificate2 oldcertificate = (from X509Certificate2 c in store.Certificates
												   where Convert.ToBase64String(c.GetCertHash()) == Convert.ToBase64String(currentHash)
												   select c).Single();

				store.Close();
				SSLCertificate certificate = new SSLCertificate();
				certificate.Hash = oldcertificate.GetCertHash();
				certificate.SerialNumber = oldcertificate.SerialNumber;
				return certificate;
			}
		}
	}
}
